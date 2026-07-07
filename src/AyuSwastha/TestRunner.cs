using System;
using System.Windows.Forms;
using AyuSwastha.Models;
using AyuSwastha.Forms;
using System.IO;

namespace TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var form = new PatientDetailForm(new Patient());
                File.WriteAllText("test_result.txt", "Success");
            }
            catch (Exception ex)
            {
                File.WriteAllText("test_result.txt", ex.ToString());
            }
        }
    }
}
