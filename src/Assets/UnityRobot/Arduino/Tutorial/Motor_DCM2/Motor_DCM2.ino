#include <UnityRobot.h>
#include <MotorModule.h>

//Motor using new DCM
#define PIN_PWM 16 //Speed control A
#define PIN_DIR1 18 //Direction A1
#define PIN_DIR2 17 //Direction A2

MotorModule motor(0);

void OnUpdate(byte id)
{
  motor.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  if(motor.updated() == true)
  {
    float speed = motor.getSpeed(2.55); // ratio = 255/100
    if(speed > 0)
    {
      digitalWrite(PIN_DIR1, HIGH);
      digitalWrite(PIN_DIR2, LOW);
      analogWrite(PIN_PWM, (int)speed);
    }
    else if(speed < 0)
    {
      digitalWrite(PIN_DIR1, LOW);
      digitalWrite(PIN_DIR2, HIGH);
      analogWrite(PIN_PWM, -(int)speed);
    }
    else
    {
      digitalWrite(PIN_DIR1, HIGH);
      digitalWrite(PIN_DIR2, HIGH);
      analogWrite(PIN_PWM, 0);
    }
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  motor.reset();
  digitalWrite(PIN_DIR1, HIGH);
  digitalWrite(PIN_DIR2, HIGH);
  analogWrite(PIN_PWM, 0);
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
  digitalWrite(PIN_DIR1, HIGH);
  digitalWrite(PIN_DIR2, HIGH);
  analogWrite(PIN_PWM, 0);
}

void OnReady(void)
{
}

void setup()
{
  pinMode(PIN_PWM, OUTPUT);
  pinMode(PIN_DIR1, OUTPUT);
  pinMode(PIN_DIR2, OUTPUT);
  
  UnityRobot.attach(CMD_UPDATE, OnUpdate);
  UnityRobot.attach(CMD_ACTION, OnAction);
  UnityRobot.attach(CMD_START, OnStart);
  UnityRobot.attach(CMD_EXIT, OnExit);
  UnityRobot.attach(CMD_READY, OnReady);
  UnityRobot.begin(57600);
}

void loop()
{
  UnityRobot.process();
}
