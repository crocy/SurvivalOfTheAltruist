namespace SurvivalOfTheAlturist {

    public interface IExport {

        string GetHeader();

        string GetExport();

    }

    public static class Export {

        public const string Separator = ";";

    }
}

