/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatScreen : Window
    {
        public Text pHpText;
        public Text pApText;
        public Text pMoveRangeText;
        public Text pVisionRangeText;
        public Button pSelectButton;

        public override void Show(Character c)
        {
            base.Show();
            pSelectButton.interactable =
                EntityManager.pInstance.pCurrentPlayers.Contains(c);
        }
    }
}
