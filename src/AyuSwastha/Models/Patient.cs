namespace AyuSwastha.Models
{
    /// <summary>A clinic patient. Adds Ayurveda-specific profile data to <see cref="Person"/>.</summary>
    public class Patient : Person
    {
        private string _patientCode;
        /// <summary>Human-friendly code shown in the UI, e.g. "P001".</summary>
        public string PatientCode 
        { 
            get => string.IsNullOrEmpty(_patientCode) && Id > 0 ? $"PAT-{Id:D4}" : _patientCode; 
            set => _patientCode = value; 
        }

        /// <summary>Ayurvedic constitution used by the recommendation engine.</summary>
        public DoshaType Prakriti { get; set; } = DoshaType.Unknown;

        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public string Lifestyle { get; set; }
        public string Notes { get; set; }
        public string PhotoPath { get; set; }

        public override string DisplayRole => "Patient";

        public override string ToString()
        {
            return string.IsNullOrEmpty(PatientCode) ? FullName : PatientCode + " — " + FullName;
        }
    }
}
