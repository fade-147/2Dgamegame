using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class huoqiu : MonoBehaviour
{
    [Header("火球设置")]
    public float speed = 10f;
    public float lifeTime = 3f; // 自动销毁时间
    public GameObject explosionEffect; // 爆炸特效

    private Rigidbody2D rb;
    private bool hasCollided = false;
    public bool pengzhuang;
    public LayerMask groundlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 设置初始速度（向前发射）
        rb.velocity =new Vector2 (transform.localScale.x*10, 0);
        // 自动销毁计时
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        pengzhuang = Physics2D.OverlapCircle(transform.position, 1, groundlayer);
        if(pengzhuang)
        {
            Destroy(gameObject);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (hasCollided) return;

        // 检查碰撞对象
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("ground"))
        {
            HandleCollision();
            Debug.Log("okeeee");
            Destroy(gameObject);

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;

        if (other.CompareTag("Enemy") || other.CompareTag("ground"))
        {
            HandleCollision();

        }
    }

    void HandleCollision()
    {
        hasCollided = true;

        //// 播放爆炸特效
        //if (explosionEffect != null)
        //{
        //    Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //}

        // 销毁火球
        Destroy(gameObject);
    }





}
