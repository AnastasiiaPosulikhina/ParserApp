using System;

namespace ParserApp
{
    public class Threat: IEquatable<Threat>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThreatSource { get; set; }
        public string InteractionObject { get; set; }
        public bool ConfidentialityBreach { get; set; }
        public bool IntegrityBreach { get; set; }
        public bool AccessBreach { get; set; }

        public Threat() { }

        public Threat(string Id, string Name, string Description, string ThreatSource, string InteractionObject, bool ConfidentialityBreach, bool IntegrityBreach, bool AccessBreach)
        {
            var s = "УБИ.";
            if (int.Parse(Id) < 10)
                this.Id = s + "00" + Id;
            else if (int.Parse(Id) < 100)
                this.Id = s + "0" + Id;
            else
                this.Id = s + Id;
            this.Name = Name;
            this.Description = Description;
            this.ThreatSource = ThreatSource;
            this.InteractionObject = InteractionObject;
            this.ConfidentialityBreach = ConfidentialityBreach;
            this.IntegrityBreach = IntegrityBreach;
            this.AccessBreach = AccessBreach;
        }

        public override string ToString()
        {
            string s = String.Format("   {0}\n" +
                "   Наименование УБИ: {1}\n" +
                "\n   Описание: {2}\n" +
                "\n   Источник угрозы: {3}\n" +
                "   Объект воздействия: {4}\n" +
                "   Нарушение конфиденциальности: {5}\n" +
                "   Нарушение целостности: {6}\n" +
                "   Нарушение доступности: {7}\n", Id, Name, Description, ThreatSource, InteractionObject, ConfidentialityBreach == true ? "да" : "нет", IntegrityBreach == true ? "да" : "нет", AccessBreach == true ? "да" : "нет");
            return s;
        }

        public bool Equals(Threat thr)
        {
            if (thr is null)
                return false;
            return Id.Equals(thr.Id) && Name.Equals(thr.Name) && Description.Equals(thr.Description) && ThreatSource.Equals(thr.ThreatSource) && InteractionObject.Equals(thr.InteractionObject) && ConfidentialityBreach.Equals(thr.ConfidentialityBreach) && IntegrityBreach.Equals(thr.IntegrityBreach) && AccessBreach.Equals(thr.AccessBreach);
        }

        public override bool Equals(object obj) => Equals(obj as Threat);
        public override int GetHashCode() => (Id).GetHashCode();
    }
}


