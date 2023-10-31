using System;

namespace FiniteStateMaсhine
{
    public struct Prop : IUnique<uint>
    {
        public uint ID { get; }
        public Type Type { get; }
        private object Value { get; set; }
        private int? MinValue { get; }
        private int? MaxValue { get; }

        public Prop(uint id, Type type, object value, int? minValue, int? maxValue)
        {
            ID = id;
            Type = type;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public void ApplyValue(object value)
        {
            if (value.GetType() != Type) throw new ArgumentException();
            switch (value)
            {
                case int i:
                {
                    var val = Convert.ToInt32(value);
                    var min = MinValue ?? int.MinValue;
                    var max = MaxValue ?? int.MaxValue;
                    Value = Math.Clamp(val, min, max);
                    break;
                }
                case float f:
                {
                    var val = Convert.ToSingle(value);
                    var min = MinValue ?? float.MinValue;
                    var max = MaxValue ?? float.MaxValue;
                    Value = Math.Clamp(val, min, max);
                    break;
                }
                default:
                    Value = value;
                    break;
            }
        }

        public int GetInt32() => Convert.ToInt32(Value);
        public float GetSingle() => Convert.ToSingle(Value);
        public string GetString() => Convert.ToString(Value);
        public bool GetBoolean() => Convert.ToBoolean(Value);
    }
}