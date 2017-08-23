using Ensage;
using Ensage.Items;

namespace VisageSharpRewrite
{
    public class PowerTreadsSwitcher
    {
        public PowerTreads PowerTreads { get; set; }

        public PowerTreadsSwitcher(PowerTreads powerTreads)
        {
            this.PowerTreads = powerTreads;
        }

        public bool IsValid
        {
            get
            {
                return this.PowerTreads != null && this.PowerTreads.IsValid;
            }
        }

        public void SwitchTo(Attribute attribute, Attribute currentAttribute, bool queue)
        {
            if (attribute == Attribute.Agility)
            {
                if (currentAttribute == Attribute.Strength)
                {
                    this.PowerTreads.UseAbility(queue);
                    this.PowerTreads.UseAbility(queue);
                }
                else if (currentAttribute == Attribute.Intelligence)
                {
                    this.PowerTreads.UseAbility(queue);
                }
            }
            else if (attribute == Attribute.Strength)
            {
                if (currentAttribute == Attribute.Intelligence)
                {
                    this.PowerTreads.UseAbility(queue);
                    this.PowerTreads.UseAbility(queue);
                }
                else if (currentAttribute == Attribute.Agility)
                {
                    this.PowerTreads.UseAbility(queue);
                }
            }
            else if (attribute == Attribute.Intelligence)
            {
                if (currentAttribute == Attribute.Agility)
                {
                    this.PowerTreads.UseAbility(queue);
                    this.PowerTreads.UseAbility(queue);
                }
                else if (currentAttribute == Attribute.Strength)
                {
                    this.PowerTreads.UseAbility(queue);
                }
            }
        }
    }
}