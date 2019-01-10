/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System.Collections.Generic;
using UnityEngine;

public class PlayerCounter : MonoBehaviour
{
    public List<Character> pCharactersInRange = new List<Character>();
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.GetComponent<Character>() != null)
        {
            other.gameObject.GetComponent<Character>().DealDamage(GetComponentInParent<Character>().GetComponent<IUniqueSpell>().Damage);
        }
    }
}
