#include <UnityRobot.h>
#include <ADCModule.h>
#include <DigitalModule.h>

ADCModule adc0(0, A0); // left IR
ADCModule adc1(1, A1); // center IR
ADCModule adc2(2, A2); // right IR
ADCModule adc3(3, A3); // portA 3
ADCModule adc4(4, A4); // portA 4
ADCModule adc5(5, A5); // portA 5
ADCModule adc6(6, A6); // portA 6
ADCModule adc7(7, A7); // portA 7

DigitalModule digital0(8, 23); // line 0
DigitalModule digital1(9, 22); // line 1
DigitalModule digital2(10, 21); // line 2
DigitalModule digital3(11, 20); // line 3
DigitalModule digital4(12, 19); // line 4
DigitalModule digital5(13, 18); // line 5
DigitalModule digital6(14, 17); // line 6
DigitalModule digital7(15, 16); // line 7
DigitalModule digital8(16, 2); // portD 2
DigitalModule digital9(17, 3); // portD 3
DigitalModule digital10(18, 4); // portD 4
DigitalModule digital11(19, 5); // portD 5
DigitalModule digital12(20, 6); // portD 6

void OnUpdate(byte id)
{
  digital0.update(id);
  digital1.update(id);
  digital2.update(id);
  digital3.update(id);
  digital4.update(id);
  digital5.update(id);
  digital6.update(id);
  digital7.update(id);
  digital8.update(id);
  digital9.update(id);
  digital10.update(id);
  digital11.update(id);
  digital12.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  digital0.action();
  digital1.action();
  digital2.action();
  digital3.action();
  digital4.action();
  digital5.action();
  digital6.action();
  digital7.action();
  digital8.action();
  digital9.action();
  digital10.action();
  digital11.action();
  digital12.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  digital0.reset();
  digital1.reset();
  digital2.reset();
  digital3.reset();
  digital4.reset();
  digital5.reset();
  digital6.reset();
  digital7.reset();
  digital8.reset();
  digital9.reset();
  digital10.reset();
  digital11.reset();
  digital12.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  adc0.flush();
  adc1.flush();
  adc2.flush();
  adc3.flush();
  adc4.flush();
  adc5.flush();
  adc6.flush();
  adc7.flush();
  
  digital0.flush();
  digital1.flush();
  digital2.flush();
  digital3.flush();
  digital4.flush();
  digital5.flush();
  digital6.flush();
  digital7.flush();
  digital8.flush();
  digital9.flush();
  digital10.flush();
  digital11.flush();
  digital12.flush();
}

void setup()
{
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
