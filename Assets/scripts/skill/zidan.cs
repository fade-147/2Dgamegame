using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class zidan : MonoBehaviour
{
    public float timer = 0;
    public float speed = 1;
    public float trackingTimer;
    public Transform trackingTransform;
    public Transform _transform;
    public float trackingRange;
    public AudioSource Finishfeidan;
    public GameObject baozha;
    void Start()
    {
        _transform = this.transform;
    }
   
    void Update()
    {
        if (trackingTimer > 0)
        {
            if (trackingTransform)
            {    //实现追踪
                var diff = (trackingTransform.position - _transform.position);
                var nowZ = _transform.rotation.eulerAngles.z;
                var toZ = (float)(Mathf.Atan2(diff.y, diff.x) * 180 / Mathf.PI);
                _transform.rotation = Quaternion.Euler(0, 0,Mathf.LerpAngle(nowZ,toZ,0.08f));   //最后这个0.02f越大，转向速度越快
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    float d = 45 * i;
                    float r = (Mathf.PI / 180) * d;
                    float x = (float)Mathf.Cos(r);
                    float y = (float)Mathf.Sin(r);
                    var hit = Physics2D.Raycast(_transform.position, new Vector2(x, y), trackingRange, 1<<8);
                    Debug.DrawLine(_transform.position, _transform.position + new Vector3(x, y, 0) * trackingRange, Color.red);
                    if (hit)
                    {
                        //print(hit.collider.name);
                        trackingTransform =hit.collider.gameObject .transform;
                    }
                }
            }
            trackingTimer -=Time.deltaTime ;
        }
        float degree = _transform.rotation.eulerAngles.z;
        //获得他移动的单位距离
        float radian = Mathf.PI / 180 * degree;
        float _x = Mathf.Cos(radian);
        float _y = Mathf.Sin(radian);
        Vector3 movement = new  Vector3(_x,_y,0 ) * speed * Time.deltaTime*60;
        _transform.position += movement;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {   
            Finishfeidan.Play();     //播放爆炸声音
            if(baozha != null)
            {           //播放爆炸特效
                Instantiate (baozha,transform .position,transform.rotation );
            }
            Destroy(this.gameObject);
        }
    }
}