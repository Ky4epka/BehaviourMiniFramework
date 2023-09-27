namespace Main.UI
{
    public interface IUINavigation
    {
        IUINavigation Parent { get; }
        IUINavigation[] Childs { get; }
        IUINavigation PrevSibling { get; }
        IUINavigation NextSibling { get; }
        IUINavigation FirstSibling { get; }
        IUINavigation LastSibling { get; }

        IUINavigation FirstChild { get; }
        IUINavigation LastChild { get; }

    }

}