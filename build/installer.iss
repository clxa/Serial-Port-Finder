; 串口设备搜索器 —— Inno Setup 安装包脚本
;
; 把「发布目录」打成单文件 Setup.exe：免管理员权限、带开始菜单/桌面快捷方式与卸载入口。
;
; 构建：双击 build\打包发布.bat（它会先编译再调用本脚本）；
;       或手动 cd build 后执行 ISCC.exe installer.iss /DMyAppVersion=1.0.0
;
; ⚠️ 本文件含中文，必须以 UTF-8 with BOM 保存，否则 Inno 会按 ANSI 读成乱码。

#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#ifndef MySourceDir
  #define MySourceDir "..\src\SerialPortDeviceFinder.WinForms\bin\Release\net48"
#endif
#ifndef ManualFile
  #define ManualFile "..\用户使用说明.md"
#endif
#ifndef OutputDirAbs
  #define OutputDirAbs "..\dist"
#endif
#ifndef ChineseIslPath
  #define ChineseIslPath "ChineseSimplified.isl"
#endif

#define MyAppName "串口设备搜索器"
#define MyAppExeName "SerialPortDeviceFinder.WinForms.exe"

[Setup]
AppId={{7C4A9E12-3B6D-4F58-9A21-5E8D0C1B7A34}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}

DefaultDirName={autopf}\SerialPortFinder
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

PrivilegesRequired=lowest

OutputDir={#OutputDirAbs}
OutputBaseFilename=串口设备搜索器-安装包-v{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "chinese"; MessagesFile: "compiler:Default.isl,{#ChineseIslPath}"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "{#MySourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb,*.xml"
Source: "{#ManualFile}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\使用说明"; Filename: "{app}\用户使用说明.md"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[Code]
{ 检测 .NET Framework 4.8（或更高）。
  Release 值 528040 是 4.8 的分界；Win10 1809 及以后基本都自带。
  同时查 64 位与 32 位注册表视图（.NET Framework 在两处都写键，
  但为稳妥起见两个视图都试一次）。 }
function IsDotNet48OrLaterInstalled(): Boolean;
var
  release: Cardinal;
begin
  Result :=
    (RegQueryDWordValue(HKLM64, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release)
      and (release >= 528040))
    or
    (RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release)
      and (release >= 528040));
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  if not IsDotNet48OrLaterInstalled() then
  begin
    Result := MsgBox(
      '未检测到 .NET Framework 4.8（或更高版本）。' + #13#10 + #13#10 +
      '本程序需要 .NET Framework 4.8 才能运行。' + #13#10 +
      'Windows 10 1809 及以后版本通常已自带；' + #13#10 +
      '较旧的系统请先安装 .NET Framework 4.8 再运行本程序。' + #13#10 + #13#10 +
      '仍要继续安装吗？',
      mbConfirmation, MB_YESNO) = IDYES;
  end;
end;

{ 说明：卸载时不会删除 devices.json —— 它由程序在运行时生成（保存的设备模板），
  不属于安装包释放的文件，Inno 默认不会碰它。这是**有意为之**：
  重装/升级不会丢掉用户辛苦配好的设备模板。 }
