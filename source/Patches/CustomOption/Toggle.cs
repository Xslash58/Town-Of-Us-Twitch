using System;

namespace TownOfUs.CustomOption
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, MultiMenu menu, string name, bool value = true) : base(id, menu, name,
            CustomOptionType.Toggle,
            value)
        {
            Format = val => (bool)val ? "On" : "Off";
        }

        protected internal bool Get()
        {
            return (bool)Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var tgl = Setting.Cast<ToggleOption>();
            tgl.CheckMark.enabled = Get();
        }
    }
}