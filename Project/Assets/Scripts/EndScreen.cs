/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UI;
using UnityEngine;

public class EndScreen : Window
{
    public override void Show(bool win)
    {
        base.Show();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);


        transform.GetChild(win ? 0 : 1).gameObject.SetActive(true);
    }
}
