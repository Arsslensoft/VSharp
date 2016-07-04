using System;
using System.Collections.Generic;
using System.Linq;
namespace VSC.AST {


    public class Switch : LoopStatement
    {
        // structure used to hold blocks of keys while calculating table switch
        sealed class LabelsRange : IComparable<LabelsRange>
        {
            public readonly long min;
            public long max;
            public readonly List<long> label_values;

            public LabelsRange(long value)
            {
                min = max = value;
                label_values = new List<long>();
                label_values.Add(value);
            }

            public LabelsRange(long min, long max, ICollection<long> values)
            {
                this.min = min;
                this.max = max;
                this.label_values = new List<long>(values);
            }

            public long Range
            {
                get
                {
                    return max - min + 1;
                }
            }

            public bool AddValue(long value)
            {
                var gap = value - min + 1;
                // Ensure the range has > 50% occupancy
                if (gap > 2 * (label_values.Count + 1) || gap <= 0)
                    return false;

                max = value;
                label_values.Add(value);
                return true;
            }

            public int CompareTo(LabelsRange other)
            {
                int nLength = label_values.Count;
                int nLengthOther = other.label_values.Count;
                if (nLengthOther == nLength)
                    return (int)(other.min - min);

                return nLength - nLengthOther;
            }
        }

      

        public Expression Expr;

        //
        // Mapping of all labels to their SwitchLabels
        //
        Dictionary<long, SwitchLabel> labels;
        Dictionary<string, SwitchLabel> string_labels;
        List<SwitchLabel> case_labels;



        SwitchLabel case_null;
        SwitchLabel case_default;
        ExplicitBlock block;



        public Switch(Expression e, ExplicitBlock block, Location l)
            : base(block)
        {
            Expr = e;
            this.block = block;
            loc = l;
        }

        public SwitchLabel ActiveLabel { get; set; }

        public ExplicitBlock Block
        {
            get
            {
                return block;
            }
        }

        public SwitchLabel DefaultLabel
        {
            get
            {
                return case_default;
            }
        }




        public List<SwitchLabel> RegisteredLabels
        {
            get
            {
                return case_labels;
            }
        }



    }
	
   
   
}