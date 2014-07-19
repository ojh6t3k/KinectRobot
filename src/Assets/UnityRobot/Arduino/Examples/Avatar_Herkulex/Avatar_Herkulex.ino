#include <UnityRobot.h>
#include <ServoModule.h>
#include <Herkulex.h>

ServoModule R_Shoulder_Pitch(0, 0, -160, 160);
ServoModule R_Shoulder_Roll(1, 1, -160, 160);
ServoModule R_Shoulder_Yaw(2, 2, -160, 160);
ServoModule R_Elbow(3, 3, -160, 160);
ServoModule L_Shoulder_Pitch(4, 4, -160, 160);
ServoModule L_Shoulder_Roll(5, 5, -160, 160);
ServoModule L_Shoulder_Yaw(6, 6, -160, 160);
ServoModule L_Elbow(7, 7, -160, 160);

unsigned long preTime = 0;

void OnUpdate(byte id)
{
  L_Shoulder_Pitch.update(id);
  L_Shoulder_Roll.update(id);
  L_Shoulder_Yaw.update(id);
  L_Elbow.update(id);
  R_Shoulder_Pitch.update(id);
  R_Shoulder_Roll.update(id);
  R_Shoulder_Yaw.update(id);
  R_Elbow.update(id);
}

// When recieved end of update
void OnAction(void)
{
  boolean updated = false;
  //TODO: Synchronizing module's action
  if(L_Shoulder_Pitch.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder_Pitch.getPin(), L_Shoulder_Pitch.getValue(512, 3.2), 0);
  }
  if(L_Shoulder_Roll.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder_Roll.getPin(), L_Shoulder_Roll.getValue(512, 3.2), 0);
  }
  if(L_Shoulder_Yaw.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder_Yaw.getPin(), L_Shoulder_Yaw.getValue(512, 3.2), 0);
  }
  if(L_Elbow.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Elbow.getPin(), L_Elbow.getValue(512, 3.2), 0);
  }
  if(R_Shoulder_Pitch.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder_Pitch.getPin(), R_Shoulder_Pitch.getValue(512, 3.2), 0);
  }
  if(R_Shoulder_Roll.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder_Roll.getPin(), R_Shoulder_Roll.getValue(512, 3.2), 0);
  }
  if(R_Shoulder_Yaw.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder_Yaw.getPin(), R_Shoulder_Yaw.getValue(512, 3.2), 0);
  }
  if(R_Elbow.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Elbow.getPin(), R_Elbow.getValue(512, 3.2), 0);
  }
  
  if(updated == true)
  { 
    unsigned long time = millis();
    unsigned long delta = millis() - preTime;
    if(delta > 1000)
      delta = 1;
    Herkulex.actionAll(delta);
    preTime = time;
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  L_Shoulder_Pitch.reset();
  L_Shoulder_Roll.reset();
  L_Shoulder_Yaw.reset();
  L_Elbow.reset();
  R_Shoulder_Pitch.reset();
  R_Shoulder_Roll.reset();
  R_Shoulder_Yaw.reset();
  R_Elbow.reset();
  Herkulex.moveOneAngle(BROADCAST_ID, 0, 0, 0);
  
  preTime = millis();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
}

void setup()
{
  Herkulex.beginSerial1(115200); //open serial
  Herkulex.reboot(BROADCAST_ID);
  delay(500);
  Herkulex.initialize(); //initialize motors
  delay(200);
  
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
