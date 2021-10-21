using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc
{
    class TableOutput
    {
        List<TableOutputColumn> columns = new List<TableOutputColumn>();

        public void AddColumn<T>(IEnumerable<T> rows, int extraRightPadding = 0)
        {
            TableOutputColumn newColumn = new TableOutputColumn();

            newColumn.Setup(rows, extraRightPadding);

            columns.Add(newColumn);
        }

        public void Print()
        {
            TableOutputColumn firstColumn = columns[0];
            columns.RemoveAt(0);

            while (firstColumn.MoveNext())
            {
                Console.Write(firstColumn.Current);

                foreach(TableOutputColumn i in columns)
                {
                    i.MoveNext();
                    Console.Write(i.Current);
                }

                Console.WriteLine();
            }

            columns.Insert(0, firstColumn);
        }
    }

    class TableOutputColumn
    {
        IEnumerator<string> enumerator;


        public void Setup<T>(IEnumerable<T> rows, int extraRightPadding)
        {
            IEnumerable<string> rawStringEnumerable = rows.Select(x => x.ToString());

            int maxLength = rawStringEnumerable.Select(x => x.Length).Max();

            enumerator = rawStringEnumerable.Select(x => x.PadRight(maxLength + extraRightPadding)).GetEnumerator();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public string Current => enumerator.Current;
    }
}
