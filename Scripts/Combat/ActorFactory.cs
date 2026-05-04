using System;
/**
Ennek segítségével sorsolják ki a véletlenszerű osztály amit a player meg enemy használ.
**/
public static class ActorFactory
{
    private static Random rng = new();

    public static ActorBase CreateRandom()
    {
        int roll = rng.Next(4);
        
        return roll switch
        {
            0 => new Fighter(),
            1 => new Archer(),
            2 => new Assassin(),
            3 => new Mage(),
            _ => new Fighter()
        };
    }
}