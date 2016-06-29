package Std
{
    public struct Nullable<T> where T : struct
    {
        private bool hasValue;
        internal T value;

        public Nullable(T value) {
            this.value = value;
            this.hasValue = true;
        }

        public bool HasValue {
            get {
                return hasValue;
                }
            }

        public T Value {
            get {
                if (!hasValue)
                    throw new InvalidOperationException("No value has been assigned");

                return value;
            }
        }
        public T GetValueOrDefault(T defaultValue) {
            return hasValue ? value : defaultValue;
        }

        public override bool Equals(object other) {
            if (!hasValue) return other == null;
            if (other == null) return false;
            return value.Equals(other);
        }

        public override int GetHashCode() {
            return hasValue ? value.GetHashCode() : 0;
        }

        public override string ToString() {
            return hasValue ? value.ToString() : "";
        }

        public static implicit operator Nullable<T>(T value) {
            return new Nullable!<T>(value);
        }

        public static explicit operator T(Nullable!<T> value) {
            return value.Value;
        }

}

}
