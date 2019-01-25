/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

namespace UI
{
    public class Window : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Show(Character c)
        {
            gameObject.SetActive(true);
        }
        public virtual void Show(bool win)
        {
            gameObject.SetActive(true);
        }
        public virtual void Show(string action)
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
