PulseModule::PulseModule(byte id, int onValue, int offValue)
{
  _id = id;
  _on = onValue;
  _off = offValue;
  reset();
}
  
void PulseModule::update(byte id)
{
  if(id == _id)
  {
    _updated = true;
    _do = false;
    UnityRobot.pop(&_duration);
  }
}
  
void PulseModule::action()
{
  if(_updated == true)
  {
    if(_duration > 0)
    {
      _do = true;
      _time = millis();
      _updated = false;
    }
  }
}
  
void PulseModule::reset()
{
  _updated = false;
  _duration = 0;
  _do = false;
}
  
int PulseModule::process()
{
  if(_do == true)
  {
    unsigned long delta = millis() - _time;
    if(_duration > (word)delta)
    {
      return _on;
    }
    else
    {
      _duration = 0;
      _do = false;
      return _off;
    }
  }
  else
  {
    return _off;
  }
}
