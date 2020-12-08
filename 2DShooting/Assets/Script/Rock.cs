using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Rock : MonoBehaviour, IDamageable<int>
{
    [SerializeField] Text lifeText;
    [SerializeField] int life;
    [SerializeField] GameObject rock;

    private void Awake()
    {
        lifeText.text = life.ToString();
    }

    public void Initialize()
    {
        life = 15;
        lifeText.text = life.ToString();
    }
    public void Damage(int damage)
    {
        life -= damage;
        lifeText.text = life.ToString();

        if(life <= 0)
        {
            var newRock = Instantiate(rock, transform.position, transform.rotation);
            newRock.GetComponent<Bound>().FirstMove(Bound.Dire.Left);
            newRock.GetComponent<Rock>().Initialize();
            newRock = Instantiate(rock, transform.position, transform.rotation);
            newRock.GetComponent<Bound>().FirstMove(Bound.Dire.Right);
            newRock.GetComponent<Rock>().Initialize();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //IDamageableを持っていたらダメージを与える
        var damageable = collision.gameObject.GetComponent<IDamageable<int>>();
        if (damageable != null)
        {
            damageable.Damage(1);
        }
    }
}
