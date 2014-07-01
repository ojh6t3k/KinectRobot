MyModule::MyModule(byte id)
{
  _id = id;
  Reset();
}
  
void MyModule::Update(byte id)
{
  if(id == _id)
  {
    UnityRobot.pop(&input_byte);
    UnityRobot.pop(&input_word);
    UnityRobot.pop(&input_short);    
  }
}
  
void MyModule::Action()
{
  output_byte += input_byte;
  output_word += input_word;
  output_short += input_short;
}
  
void MyModule::Reset()
{
  output_byte = 0;
  output_word = 0;
  output_short = 0;
}
  
void MyModule::Flush()
{
  UnityRobot.select(_id);
  UnityRobot.push(output_byte);
  UnityRobot.push(output_word);
  UnityRobot.push(output_short);
  UnityRobot.flush();
}
