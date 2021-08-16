using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class bus : MonoBehaviour{

    public WheelCollider[] colls = new WheelCollider[4]; //바퀴가 돌아가는 걸 표현하기위한 메쉬
    public Transform[] tires = new Transform[4];
    public TextMeshProUGUI speedT;
    public string season = ""; 

    Rigidbody rb;
    AudioSource sound;

    Vector3 pos;
    Quaternion rot;
    
    private float velocity;
    public float accel; //0.01씩 증가가 기본, 유니티 씬에서 받아오는 값 (맵마다 다른 값)
    public static float speed;
    public static int sss; // 스크린에 비춰지는 속도값

    private float timecheck; //속도 떨어뜨릴때 타임체크
    private bool breaks = false;
    
    private bool icecheck = false;
    private bool puddle = false;
    private bool out_check = false;

    private float outTime = 0f;
    public static bool pause = false;
    public static bool isOut = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>(); //리지드바디를 받아온다.
        sound = GetComponent<AudioSource>(); 
        //rb.centerOfMass = new Vector3(0, 0, 0); //무게중심을 가운데로 맞춰서 안정적으로 주행하도록 한다.*/
        velocity = 0;
        speed = 0;
        breaks = false;
        isOut = false;
        icecheck = false;
        out_check = false;
        outTime = 0f;
        pause = false;
    }

    private void Update()
    {   
        //Debug.Log();
        // 일직선으로 세워졌을때 아웃시키는 부분
        if(Math.Abs(transform.eulerAngles.x) < 100 && Math.Abs(transform.eulerAngles.x) > 80){
            if (!out_check){
                breaks = true;
                outTime = Time.time;
                out_check = true;
            }
        }
        else out_check = false;

        if (out_check && Time.time - outTime >= 2) isOut = true;

        // 일시정지가 아닐때 버스 움직임
        if (!pause){

            //버스 이동
            transform.Translate(Vector3.forward * -speed * Time.deltaTime * 0.5f);

            //버스 바퀴 회전
            for (int i=0;i<4;i++)  {
                rotateWheel(i, season);
            }
    
            //버스 차체 회전
            rotateBody(season);

            //급정거
            if (Math.Abs(speed) > 0 && Input.GetKeyDown(KeyCode.Space)){
                breaks = true;
                rb.AddRelativeForce(new Vector3(0, -1, 0) * 1000 * Math.Abs(speed));
                rb.AddRelativeForce(new Vector3(0, 0, -1) * 50000 * Math.Abs(speed));
                //rb.AddRelativeForce(new Vector3(0, 0, 1) * 30000 * Math.Abs(speed));
            }

            if (breaks){
                for(int i=0;i<4;i++) {
                    colls[i].brakeTorque = Mathf.Infinity;
                }
                if (speed > 0.1f) speed -=0.2f;
                else if (speed < -0.1f) speed +=0.2f;
                else {
                    speed=0;
                    breaks = false;
                }
            }
            
            // 속도 text
            sss = (int)(speed * 10);
            speedT.text = sss.ToString();

            //1초 내에 속도변화가 일어나지 않으면 0.1초에 1씩 줄어듦 //귀찮아서 2초로 늘림
            if (speed != 0 && Time.time - timecheck >= 0.2) {
                timecheck = Time.time;
                if (speed < 0.1f && speed > -0.1f) speed = 0;
                else if (speed > 0) speed-=0.1f;
                else if (speed < 0) speed+=0.1f;
            }

            //마우스오버시 수행내용
            if (mouse.speedChange && !breaks)
            {
                changeSpeed();
            }
            else {
                velocity = 0;
            }
        }
        
    }

    void changeSpeed(){ //속도변화

        if (mouse.crossP > 0) {
            velocity += 0.01f * mouse.crossP;
        }
        else if (mouse.crossP < 0){
            velocity += 0.01f * mouse.crossP;
        }

        if (velocity >= 1) {
            speed += accel;
            velocity = 0;
            timecheck = Time.time;
        }
        else if (velocity <= -1){
            speed -= accel;
            velocity = 0;
            timecheck = Time.time;
        }
    }

    // 차체 회전
    void rotateBody(string s){

        switch(s){

            case "winter":
                if(!icecheck){
                    if (Math.Abs(speed) < 2) transform.Rotate(Vector3.up * 0.17f * speed * Input.GetAxis("Horizontal") );
                    else if (Math.Abs(speed) < 7)  transform.Rotate(Vector3.up * 0.15f * speed *Input.GetAxis("Horizontal"));
                    else if (speed > 0) transform.Rotate(Vector3.up * 0.15f * 7 *Input.GetAxis("Horizontal"));
                    else if (speed < 0) transform.Rotate(Vector3.up * 0.15f * -7 *Input.GetAxis("Horizontal"));
                }
                break;

            case "summer":
                if(!puddle){
                    if (Math.Abs(speed) < 2) transform.Rotate(Vector3.up * 0.17f * speed * Input.GetAxis("Horizontal") );
                    else if (Math.Abs(speed) < 7)  transform.Rotate(Vector3.up * 0.15f * speed *Input.GetAxis("Horizontal"));
                    else if (speed > 0) transform.Rotate(Vector3.up * 0.15f * 7 *Input.GetAxis("Horizontal"));
                    else if (speed < 0) transform.Rotate(Vector3.up * 0.15f * -7 *Input.GetAxis("Horizontal"));
                }
                else{
                    if (Math.Abs(speed) < 2) transform.Rotate(Vector3.up * 0.17f * -speed * Input.GetAxis("Horizontal") );
                    else if (Math.Abs(speed) < 7)  transform.Rotate(Vector3.up * 0.15f * -speed *Input.GetAxis("Horizontal"));
                    else if (speed > 0) transform.Rotate(Vector3.up * 0.15f * -7 *Input.GetAxis("Horizontal"));
                    else if (speed < 0) transform.Rotate(Vector3.up * 0.15f * 7 *Input.GetAxis("Horizontal"));
                }
                break;

            default:
                if (Math.Abs(speed) < 2) transform.Rotate(Vector3.up * 0.17f * speed * Input.GetAxis("Horizontal") );
                else if (Math.Abs(speed) < 7)  transform.Rotate(Vector3.up * 0.15f * speed *Input.GetAxis("Horizontal"));
                else if (speed > 0) transform.Rotate(Vector3.up * 0.15f * 7 *Input.GetAxis("Horizontal"));
                else if (speed < 0) transform.Rotate(Vector3.up * 0.15f * -7 *Input.GetAxis("Horizontal"));
                break;
        }

    }

    void rotateWheel(int wheel, string s){
         
        tires[wheel].Rotate(Vector3.right * -speed);

        if (colls[wheel].rpm > 1) {
            colls[wheel].brakeTorque = Mathf.Infinity;
        }
        

        switch(s){

            case "winter":
            if (wheel%2==1 && !icecheck){
                
                if (Math.Abs(speed) < 5)  colls[wheel].steerAngle = 3 * Input.GetAxis("Horizontal") * Math.Abs(speed);
                else colls[wheel].steerAngle = 5 * Input.GetAxis("Horizontal") * 3;
                
                //Vector3 rot;
                colls[wheel].GetWorldPose(out pos, out rot);
                //rot.x = tires[wheel].eulerAngles.x + 
                //if (tires[wheel].eulerAngles.x <= 0) tires[wheel].rotation = Quaternion.Euler(tires[wheel].eulerAngles.x,rotation.eulerAngles.y,tires[wheel].eulerAngles.z);
                //else tires[wheel].rotation = Quaternion.Euler(tires[wheel].eulerAngles.x,-rotation.eulerAngles.y,tires[wheel].eulerAngles.z);
                tires[wheel].rotation = rot;
            }
            break;

            case "summer":
            if (wheel%2==1 && !puddle){
                
                if (Math.Abs(speed) < 5)  colls[wheel].steerAngle = 3 * Input.GetAxis("Horizontal") * Math.Abs(speed);
                else colls[wheel].steerAngle = 5 * Input.GetAxis("Horizontal") * 3;
                
            }
            else if (wheel%2==1 && puddle){
                
                if (Math.Abs(speed) < 5)  colls[wheel].steerAngle = 3 * -Input.GetAxis("Horizontal") * Math.Abs(speed);
                else colls[wheel].steerAngle = 5 * -Input.GetAxis("Horizontal") * 3;
                
            }
            //Vector3 rot;
            colls[wheel].GetWorldPose(out pos, out rot);
            //rot = tires[i].eulerAngles + rotation.eulerAngles;
            tires[wheel].rotation = rot;
            break;

            default:
            if (wheel%2==1 && !icecheck){
                
                if (Math.Abs(speed) < 5)  colls[wheel].steerAngle = 3 * Input.GetAxis("Horizontal") * Math.Abs(speed);
                else colls[wheel].steerAngle = 5 * Input.GetAxis("Horizontal") * 3;
                //Vector3 rot;
                colls[wheel].GetWorldPose(out pos, out rot);
                //rot.x = tires[wheel].eulerAngles.x + 
                //if (tires[wheel].eulerAngles.x <= 0) tires[wheel].rotation = Quaternion.Euler(tires[wheel].eulerAngles.x,rotation.eulerAngles.y,tires[wheel].eulerAngles.z);
                //else tires[wheel].rotation = Quaternion.Euler(tires[wheel].eulerAngles.x,-rotation.eulerAngles.y,tires[wheel].eulerAngles.z);
                tires[wheel].rotation = rot;
            }
            break;
        }

            
    }


    void OnTriggerEnter(Collider col){
        if (col.gameObject.CompareTag("gravel")){
            accel = 0.04f;
        }

        if (col.gameObject.CompareTag("gameOver")){
            isOut = true;
        }
    }
    void OnTriggerStay(Collider col) {
        if(col.gameObject.CompareTag("BlackIce"))
        {
            icecheck = true;
        }
        else if(col.gameObject.CompareTag("puddle"))
        {
            puddle = true;
        }
    }
    void OnTriggerExit(Collider col){
        if(col.gameObject.CompareTag("BlackIce")){
            icecheck = false;
        }
        else if(col.gameObject.CompareTag("puddle"))
        {
            puddle = false;
        }
        if (col.gameObject.CompareTag("gravel")){
            accel = 0.02f;
        }
    }
    

    void OnCollisionEnter(Collision col){
        if (col.collider.CompareTag("gameOver")){
            isOut = true;
        }
    }

}