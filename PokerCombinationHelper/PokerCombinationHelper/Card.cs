﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PokerCombinationHelper;
using Checker;

namespace Cards
{
    public enum CardSuit
    {
        [Description("Черви")] Hearts = 1,
        [Description("Буби")] Diamonds = 2,
        [Description("Крести")] Clubs = 3,
        [Description("Пики")] Spades = 4,

    }
    public enum CardValue
    {
        [Description("Двойка")] Two = 2,
        [Description("Тройка")] Three = 3,
        [Description("Четверка")] Four = 4,
        [Description("Пятерка")] Five = 5,
        [Description("Шестерка")] Six = 6,
        [Description("Семерка")] Seven = 7,
        [Description("Восьмерка")] Eight = 8,
        [Description("Девятка")] Nine = 9,
        [Description("Десятка")] Ten = 10,
        [Description("Валет")] Jack = 11,
        [Description("Дама")] Queen = 12,
        [Description("Король")] King = 13,
        [Description("Туз")] Ace = 14,
    }

    public class Card
    {
        public CardValue Value;
        public CardSuit Suit;
        public static Card[] Deck = Card.GetDeck();

        public static Card[] GetDeck()
        {
            Card[] deckMassive = new Card[52];
            int k = 0;

            for (int i = 2; i <= 14; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    Card card1 = new Card();
                    card1.Suit = (CardSuit)j;
                    card1.Value = (CardValue)i;
                    deckMassive[k] = card1;
                    k++;
                }
            }
            return deckMassive;
        }
        public static Card GetRandomCard()
        {
            var rnd = new Random();
            Card rndCard = new Card();

            int rndNumber = rnd.Next(0, 51);
            Deck[rndNumber] = rndCard;


            return Card { Value = (CardValue)rnd.Next(2, 14), Suit = (CardSuit)rnd.Next(1, 4) };
        }

        //public static Card GetRandomCard()
        //{
        //    var rnd = new Random();
        //    return new Card { Value = (CardValue)rnd.Next(2, 14), Suit = (CardSuit)rnd.Next(1, 4) };
        //}

        public static Card[] GetCards(int count)
        {
            Card[] boardCards = new Card[count];

            for (int i = 0; i < boardCards.Length; i++)
            {
                boardCards[i] = Card.GetRandomCard();
            }

            return boardCards;
        }


    }

}
