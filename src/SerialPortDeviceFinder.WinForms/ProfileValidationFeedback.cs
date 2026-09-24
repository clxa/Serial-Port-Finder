using System;
using System.Collections.Generic;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 将跨模板校验结果转换为可直接显示在主窗体上的反馈。
    /// </summary>
    public sealed class ProfileValidationFeedback
    {
        private ProfileValidationFeedback(bool isValid, bool canStartSearch, string message)
        {
            IsValid = isValid;
            CanStartSearch = canStartSearch;
            Message = message;
        }

        public bool IsValid { get; }

        public bool CanStartSearch { get; }

        public string Message { get; }

        public static ProfileValidationFeedback Create(IReadOnlyList<DeviceProfile> profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            var errors = DeviceProfileValidator.ValidateProfiles(profiles);
            if (profiles.Count == 0)
            {
                return new ProfileValidationFeedback(true, false, "尚未保存设备模板，请新增或加载模板。");
            }

            return errors.Count == 0
                ? new ProfileValidationFeedback(true, true, "所有模板检查通过，可以开始搜索。")
                : new ProfileValidationFeedback(false, false, "所有模板需要修正：" + string.Join("  ", errors));
        }
    }
}
