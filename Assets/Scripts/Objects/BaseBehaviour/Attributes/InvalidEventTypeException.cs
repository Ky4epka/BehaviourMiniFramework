using System;

namespace Main.Events
{
    public class InvalidEventTypeException: TypeAccessException
    {
        public InvalidEventTypeException(Type inputType): 
            base(string.Concat("Event type must be inherited from a '",
                                    typeof(IEventData).FullName,
                                    "'! Input value: ",
                                    inputType?.FullName))
        {
        }
    }
}