namespace SearchStringParser {
    public enum SearchModificator {
        None, Include, Exclude, Field, Group
    }
    enum SearchStringParseState {
        Calculating, Completed
    }
}
