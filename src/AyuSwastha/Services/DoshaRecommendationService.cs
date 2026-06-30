using System.Collections.Generic;
using AyuSwastha.Models;

namespace AyuSwastha.Services
{
    /// <summary>Bundle of Ayurvedic guidance produced for a given constitution.</summary>
    public class DoshaRecommendation
    {
        public DoshaType Dosha { get; set; }
        public string Summary { get; set; }
        public IReadOnlyList<string> RecommendedHerbs { get; set; } = new List<string>();
        public IReadOnlyList<string> RecommendedTherapies { get; set; } = new List<string>();
        public IReadOnlyList<string> DietTips { get; set; } = new List<string>();
        public IReadOnlyList<string> LifestyleTips { get; set; } = new List<string>();
    }

    /// <summary>
    /// ★ Innovative feature — the Dosha-Based Recommendation Engine.
    /// Maps a patient's Prakriti (Vata/Pitta/Kapha and duals) to suggested herbs,
    /// therapies, diet, and lifestyle guidance rooted in Ayurvedic principles.
    /// It can also infer the dominant Dosha from a short questionnaire tally.
    /// </summary>
    public class DoshaRecommendationService
    {
        /// <summary>
        /// Determine the constitution from three questionnaire tallies. Two clearly
        /// dominant scores produce a dual type; three near-equal scores are Tridoshic.
        /// </summary>
        public DoshaType Determine(int vata, int pitta, int kapha)
        {
            int max = System.Math.Max(vata, System.Math.Max(pitta, kapha));
            if (max == 0) return DoshaType.Unknown;

            bool v = vata == max, p = pitta == max, k = kapha == max;

            if (v && p && k) return DoshaType.Tridoshic;
            if (v && p) return DoshaType.VataPitta;
            if (p && k) return DoshaType.PittaKapha;
            if (v && k) return DoshaType.VataKapha;
            if (v) return DoshaType.Vata;
            if (p) return DoshaType.Pitta;
            return DoshaType.Kapha;
        }

        public DoshaRecommendation GetRecommendation(DoshaType dosha)
        {
            switch (dosha)
            {
                case DoshaType.Vata:
                    return new DoshaRecommendation
                    {
                        Dosha = dosha,
                        Summary = "Vata (air + ether): keep warm, grounded, and well-nourished.",
                        RecommendedHerbs = new[] { "Ashwagandha", "Bala", "Dashamula", "Shatavari" },
                        RecommendedTherapies = new[] { "Abhyanga", "Shirodhara", "Basti" },
                        DietTips = new[] { "Warm, cooked, moist meals", "Favour sweet, sour, salty tastes", "Avoid raw/cold foods" },
                        LifestyleTips = new[] { "Regular routine and early sleep", "Gentle oil self-massage", "Avoid overexertion" }
                    };
                case DoshaType.Pitta:
                    return new DoshaRecommendation
                    {
                        Dosha = dosha,
                        Summary = "Pitta (fire + water): stay cool, calm, and avoid excess heat.",
                        RecommendedHerbs = new[] { "Amalaki", "Guduchi", "Neem", "Shatavari" },
                        RecommendedTherapies = new[] { "Shirodhara", "Sitali Pranayama", "Cooling herbal packs" },
                        DietTips = new[] { "Cooling foods; sweet, bitter, astringent tastes", "Limit spicy, oily, fried food", "Stay hydrated" },
                        LifestyleTips = new[] { "Avoid midday sun and overheating", "Moderate, non-competitive exercise", "Practice calming activities" }
                    };
                case DoshaType.Kapha:
                    return new DoshaRecommendation
                    {
                        Dosha = dosha,
                        Summary = "Kapha (earth + water): keep active, light, and stimulated.",
                        RecommendedHerbs = new[] { "Triphala", "Trikatu", "Guggulu", "Punarnava" },
                        RecommendedTherapies = new[] { "Udvartana (dry massage)", "Swedana", "Vamana (under supervision)" },
                        DietTips = new[] { "Light, warm, spiced meals", "Favour pungent, bitter, astringent tastes", "Reduce dairy, sweets, heavy food" },
                        LifestyleTips = new[] { "Vigorous daily exercise", "Rise early; avoid daytime sleep", "Seek variety and stimulation" }
                    };
                case DoshaType.VataPitta:
                    return Combine(dosha, GetRecommendation(DoshaType.Vata), GetRecommendation(DoshaType.Pitta),
                        "Vata-Pitta: balance grounding with cooling; adjust to the season.");
                case DoshaType.PittaKapha:
                    return Combine(dosha, GetRecommendation(DoshaType.Pitta), GetRecommendation(DoshaType.Kapha),
                        "Pitta-Kapha: favour cooling and light, stimulating routines.");
                case DoshaType.VataKapha:
                    return Combine(dosha, GetRecommendation(DoshaType.Vata), GetRecommendation(DoshaType.Kapha),
                        "Vata-Kapha: keep warm and active; avoid cold and heaviness.");
                case DoshaType.Tridoshic:
                    return new DoshaRecommendation
                    {
                        Dosha = dosha,
                        Summary = "Tridoshic: a balanced constitution — maintain seasonal moderation.",
                        RecommendedHerbs = new[] { "Triphala", "Ashwagandha", "Amalaki" },
                        RecommendedTherapies = new[] { "Abhyanga", "Seasonal Panchakarma" },
                        DietTips = new[] { "Eat with the seasons", "Fresh, balanced, moderate portions" },
                        LifestyleTips = new[] { "Consistent daily routine", "Balanced activity and rest" }
                    };
                default:
                    return new DoshaRecommendation
                    {
                        Dosha = DoshaType.Unknown,
                        Summary = "Constitution not yet assessed. Complete the Prakriti questionnaire for tailored guidance.",
                        RecommendedHerbs = new List<string>(),
                        RecommendedTherapies = new List<string>(),
                        DietTips = new List<string>(),
                        LifestyleTips = new List<string>()
                    };
            }
        }

        private static DoshaRecommendation Combine(DoshaType dosha, DoshaRecommendation a, DoshaRecommendation b, string summary)
        {
            return new DoshaRecommendation
            {
                Dosha = dosha,
                Summary = summary,
                RecommendedHerbs = Merge(a.RecommendedHerbs, b.RecommendedHerbs),
                RecommendedTherapies = Merge(a.RecommendedTherapies, b.RecommendedTherapies),
                DietTips = Merge(a.DietTips, b.DietTips),
                LifestyleTips = Merge(a.LifestyleTips, b.LifestyleTips)
            };
        }

        private static IReadOnlyList<string> Merge(IReadOnlyList<string> x, IReadOnlyList<string> y)
        {
            var set = new List<string>();
            foreach (var s in x) if (!set.Contains(s)) set.Add(s);
            foreach (var s in y) if (!set.Contains(s)) set.Add(s);
            return set;
        }
    }
}
