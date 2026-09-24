<#
  串口设备搜索器 —— 一键打包发布

  产出：
    dist\串口设备搜索器-v<版本>-<日期>.zip        解压即用
    dist\串口设备搜索器-安装包-v<版本>.exe         需已装 Inno Setup 6（免管理员权限）
    dist\github-release\                          ASCII 文件名副本，专供上传 GitHub Release
      （GitHub 会剥掉附件名里的非 ASCII 字符，中文名会被改坏，所以要这份副本）

  用法：双击 build\打包发布.bat，或
        powershell -ExecutionPolicy Bypass -File build\build-release.ps1 [-Version x.y.z] [-SkipInstaller]

  版本号默认取自 AssemblyInfo.cs，避免两处不一致。
  本文件含中文，必须保存为 UTF-8 with BOM。
#>
[CmdletBinding()]
param(
    # 留空 = 自动从 AssemblyInfo.cs 读取
    [string]$Version = '',
    # 只打 zip、跳过安装包（适合本机没装 Inno Setup 时）
    [switch]$SkipInstaller,
    # 保留 .pdb 调试符号（默认不放进发布包）
    [switch]$KeepPdb
)

$ErrorActionPreference = 'Stop'
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

function Step($text) { Write-Host "`n=== $text ===" -ForegroundColor Cyan }
function Ok($text) { Write-Host "    [完成] $text" -ForegroundColor Green }
function Warn($text) { Write-Host "    [注意] $text" -ForegroundColor Yellow }
function Fail($text) { Write-Host "`n[失败] $text" -ForegroundColor Red }

$buildDir = $PSScriptRoot
$root = Split-Path -Parent $buildDir
$project = Join-Path $root 'src\SerialPortDeviceFinder.WinForms\SerialPortDeviceFinder.WinForms.csproj'
$releaseDir = Join-Path $root 'src\SerialPortDeviceFinder.WinForms\bin\Release\net48'
$distDir = Join-Path $root 'dist'
$manualFile = Join-Path $root '用户使用说明.md'
$issFile = Join-Path $buildDir 'installer.iss'
$chineseIsl = Join-Path $buildDir 'ChineseSimplified.isl'
$appFolderName = '串口设备搜索器'

Write-Host "串口设备搜索器 打包发布" -ForegroundColor White
Write-Host "项目根目录：$root" -ForegroundColor DarkGray

if (-not $Version) {
    $Version = '1.0.0'
    $assemblyInfo = Join-Path $root 'src\SerialPortDeviceFinder.WinForms\Properties\AssemblyInfo.cs'
    if (Test-Path $assemblyInfo) {
        $match = Select-String -Path $assemblyInfo -Pattern 'AssemblyInformationalVersion\("([^"]+)"\)' -ErrorAction SilentlyContinue |
            Select-Object -First 1
        if ($match) { $Version = $match.Matches[0].Groups[1].Value }
    }
}
Write-Host "版本号：$Version" -ForegroundColor DarkGray

$date = Get-Date -Format 'yyyy-MM-dd'

Step '1/6 检查编译环境'
# 注意：Windows 自带的 Windows PowerShell 5.1 不支持 `?.` 这类新语法，
# 所以这里必须写成"先取值、再判断"的形式，否则整个脚本会解析失败。
$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
$dotnet = $null
if ($dotnetCmd) { $dotnet = $dotnetCmd.Source }
if (-not $dotnet) {
    foreach ($candidate in @(
            'C:\Program Files\dotnet\dotnet.exe',
            'C:\Program Files (x86)\dotnet\dotnet.exe')) {
        if (Test-Path $candidate) { $dotnet = $candidate; break }
    }
}
if (-not $dotnet) {
    Fail '未找到 dotnet 命令。请安装 .NET SDK（或 Visual Studio 的「.NET 桌面开发」工作负载）后重试。'
    exit 1
}
Ok "dotnet: $dotnet"

Step '2/6 编译 Release'
& $dotnet build $project -c Release --nologo -v minimal
if ($LASTEXITCODE -ne 0) {
    Fail '编译失败，打包中止。请先修复编译错误。'
    exit 1
}
Ok '编译通过'
if (-not (Test-Path $releaseDir)) {
    Fail "未找到发布目录：$releaseDir"
    exit 1
}

Step '3/6 组装发布目录'
$stage = Join-Path $env:TEMP ('spf-pkg-' + [Guid]::NewGuid().ToString('N'))
$appDir = Join-Path $stage $appFolderName
New-Item -ItemType Directory -Path $appDir -Force | Out-Null

$exclude = if ($KeepPdb) { @() } else { @('*.pdb', '*.xml') }
Get-ChildItem -Path $releaseDir -File |
    Where-Object { $name = $_.Name; -not ($exclude | Where-Object { $name -like $_ }) } |
    ForEach-Object { Copy-Item $_.FullName $appDir -Force }

if (Test-Path $manualFile) {
    Copy-Item $manualFile $appDir -Force
    Ok '已附上《用户使用说明.md》'
} else {
    Warn '未找到《用户使用说明.md》，发布包将不含使用说明'
}

$files = Get-ChildItem $appDir -File
Ok ("发布目录包含 $($files.Count) 个文件：" + (($files | Sort-Object Name | ForEach-Object Name) -join '、'))

Step '4/6 生成发布压缩包'
if (-not (Test-Path $distDir)) { New-Item -ItemType Directory -Path $distDir -Force | Out-Null }
$zipPath = Join-Path $distDir "串口设备搜索器-v$Version-$date.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path $appDir -DestinationPath $zipPath -Force
Ok ("$zipPath  （{0:N0} KB）" -f ((Get-Item $zipPath).Length / 1KB))

Step '5/6 生成安装包'
$setupPath = $null
if ($SkipInstaller) {
    Warn '按要求跳过安装包（-SkipInstaller）'
} else {
    $iscc = $null
    foreach ($candidate in @(
            (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe'),
            'C:\Program Files (x86)\Inno Setup 6\ISCC.exe',
            'C:\Program Files\Inno Setup 6\ISCC.exe')) {
        if (Test-Path $candidate) { $iscc = $candidate; break }
    }

    if (-not $iscc) {
        $found = Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall',
            'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall' -ErrorAction SilentlyContinue |
            ForEach-Object { (Get-ItemProperty $_.PSPath -ErrorAction SilentlyContinue).InstallLocation } |
            Where-Object { $_ -and (Test-Path (Join-Path $_ 'ISCC.exe')) } |
            Select-Object -First 1
        if ($found) { $iscc = Join-Path $found 'ISCC.exe' }
    }

    if (-not $iscc) {
        Warn '未找到 Inno Setup 的编译器 ISCC.exe，已跳过安装包。'
        Write-Host '          安装方式（免管理员权限）：winget install JRSoftware.InnoSetup' -ForegroundColor DarkGray
        Write-Host '          装好后重新运行本脚本即可生成安装包。' -ForegroundColor DarkGray
    } else {
        Ok "ISCC: $iscc"
        # 注意：不要用 $args 这个名字 —— 它是 PowerShell 的内置自动变量。
        $isccArgs = @(
            $issFile,
            "/DMyAppVersion=$Version",
            "/DMySourceDir=$releaseDir",
            "/DManualFile=$manualFile",
            "/DOutputDirAbs=$distDir",
            "/DChineseIslPath=$chineseIsl"
        )
        & $iscc @isccArgs
        if ($LASTEXITCODE -ne 0) {
            Warn "安装包编译失败（ISCC 退出码 $LASTEXITCODE）。zip 仍然可用。"
        } else {
            $setupPath = Join-Path $distDir "串口设备搜索器-安装包-v$Version.exe"
            if (Test-Path $setupPath) {
                Ok ("$setupPath  （{0:N0} KB）" -f ((Get-Item $setupPath).Length / 1KB))
            }
        }
    }
}

Step '6/6 生成 Release 上传副本'
$relDir = Join-Path $distDir 'github-release'
if (Test-Path $relDir) { Remove-Item $relDir -Recurse -Force -ErrorAction SilentlyContinue }
New-Item -ItemType Directory -Path $relDir -Force | Out-Null

$portableDst = Join-Path $relDir "SerialPortFinder-v$Version-portable.zip"
Copy-Item $zipPath $portableDst -Force
Ok ("$portableDst  （{0:N0} KB）" -f ((Get-Item $portableDst).Length / 1KB))

if ($setupPath -and (Test-Path $setupPath)) {
    $setupDst = Join-Path $relDir "SerialPortFinder-Setup-v$Version.exe"
    Copy-Item $setupPath $setupDst -Force
    Ok ("$setupDst  （{0:N0} KB）" -f ((Get-Item $setupDst).Length / 1KB))
} else {
    Warn '未生成安装包，Release 副本里只有 zip'
}

Remove-Item $stage -Recurse -Force -ErrorAction SilentlyContinue

Step '打包完成'
Write-Host '  dist\ （中文名，发给同事 / 离线分发）：' -ForegroundColor Cyan
Get-ChildItem $distDir -File | Sort-Object Name | ForEach-Object {
    Write-Host ("    {0}    {1:N0} KB" -f $_.Name, ($_.Length / 1KB)) -ForegroundColor White
}
Write-Host '  dist\github-release\ （ASCII 名，上传 GitHub Release 用）：' -ForegroundColor Cyan
Get-ChildItem $relDir -File | Sort-Object Name | ForEach-Object {
    Write-Host ("    {0}    {1:N0} KB" -f $_.Name, ($_.Length / 1KB)) -ForegroundColor White
}
Write-Host ''
Write-Host '发给使用者的做法：' -ForegroundColor Cyan
Write-Host '  · 只想用软件 → 把 zip 发过去，对方解压后双击 exe 即可' -ForegroundColor Gray
Write-Host '  · 想要安装感  → 把「安装包-v*.exe」发过去，双击安装' -ForegroundColor Gray
Write-Host ''
Write-Host '提示：dist\ 已加入 .gitignore，不进版本库。对外发布走 GitHub Releases：' -ForegroundColor DarkGray
Write-Host '      仓库页 → Releases → Draft a new release → 标签填 v<版本> →' -ForegroundColor DarkGray
Write-Host '      上传 dist\github-release\ 里的两个文件。' -ForegroundColor DarkGray
