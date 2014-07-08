#include <UnityRobot.h>
#include <DigitalModule.h>
#include <ADCModule.h>
#include <PWMModule.h>

ADCModule px_axis(0, A0);
ADCModule mx_axis(1, A1);
ADCModule py_axis(2, A2);
ADCModule my_axis(3, A3);
ADCModule r_color(4, A4);
ADCModule g_color(5, A5);
ADCModule b_color(6, A6);
ADCModule dial(7, A7);

DigitalModule button(8, 8);
DigitalModule b_led(9, 9);
PWMModule r_led(10, 16);

void OnUpdate(byte id)
{
  button.update(id);
  b_led.update(id);
  r_led.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  button.action();
  b_led.action();
  r_led.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  button.reset();
  b_led.reset();
  r_led.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  px_axis.flush();
  mx_axis.flush();
  py_axis.flush(); 
  my_axis.flush();
  r_color.flush();
  g_color.flush();
  b_color.flush();
  button.flush();
  dial.flush();
  b_led.flush();
}

void setup()
{
  button.begin();
  b_led.begin();
  r_led.begin();
  
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
