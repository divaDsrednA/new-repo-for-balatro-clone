public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public class CardData
{
    public int rank;
    public Suit suit;

    public CardData(int rank, Suit suit)
    {
        this.rank = rank;
        this.suit = suit;
    }
}

