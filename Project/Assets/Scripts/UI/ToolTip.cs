using System.Collections.Specialized;
using UnityEngine.UI;

namespace UI
{
    public class ToolTip : Window
    {
        public Text pNameText;
        public Text pRangeText;
        public Text pDamageText;
        public Text pCooldownText;
        public Text pDescriptionText;


        public override void Show(string action)
        {
            base.Show(action);
            Character c = GameManager.pInstance.pActiveCharacter;
            switch (action)
            {
                case ("FireBall"):
                    pNameText.text = c.SpellName;
                    pRangeText.text = c.Range.ToString();
                    pDamageText.text = c.Damage.ToString();
                    pCooldownText.text = c.Cooldown.ToString();
                    pDescriptionText.text = c.Description;
                    break;
                case ("Unique"):
                    pNameText.text = c.pUniqueSpell.SpellName;
                    pRangeText.text = c.pUniqueSpell.Range.ToString();
                    pDamageText.text = c.pUniqueSpell.Damage.ToString();
                    pCooldownText.text = c.pUniqueSpell.Cooldown.ToString();
                    pDescriptionText.text = c.pUniqueSpell.Description;
                    break;
            }
        }
    }
}