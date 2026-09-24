namespace DemoClassLibrary
{
    public class DemoClass
    {
        public string CreateAndFindStringOnBuffer()
        {
            string buffer = string.Empty;
            string ret = string.Empty;

            for (int i = 0; i < 10_000; i++)
            {
                buffer = buffer + "Row " + i + "\n";
            }

            for (int i = 0; i < buffer.Length - 7; i++)
            {
                string sub = buffer.Substring(i, 7);
                if (sub == "Row 100")
                {
                    ret = sub;
                    break;
                }
            }

            return ret;
        }
    }
}
