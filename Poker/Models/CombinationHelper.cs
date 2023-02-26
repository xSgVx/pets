﻿using CardGameBase;
using CardGameBase.Models.Comparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Models
{
    public enum PokerCombinations
    {
        [Description("Флеш рояль")] RoyalFlush = 10,
        [Description("Стрит флеш")] StraightFlush = 9,
        [Description("Карэ")] FourOfAKind = 8,
        [Description("Фулл хаус")] FullHouse = 7,
        [Description("Флеш")] Flush = 6,
        [Description("Стрит")] Straight = 5,
        [Description("Сет (Тройка)")] ThreeOfAKind = 4,
        [Description("Две пары")] TwoPair = 3,
        [Description("Пара")] Pair = 2,
        [Description("Старшая карта")] HighCard = 1,
        [Description("Ничья")] Draw = 0,
    }

    internal class CombinationHelper
    {
        public PokerCombinations Combination { get; private set; }
        public IEnumerable<ICard> WinnerCards { get; private set; }

        private Dictionary<CardSuit, List<ICard>> _dictByCardSuit = new();
        private Dictionary<CardValue, List<ICard>> _dictByCardValue = new();
        private IEnumerable<ICard> _royalFlush;
        private IEnumerable<ICard> _straightFlush;
        private IEnumerable<ICard> _fourOfAKind;
        private IEnumerable<ICard> _fullHouse;
        private IEnumerable<ICard> _flush;
        private IEnumerable<ICard> _straight;
        private IEnumerable<ICard> _threeOfAKind;
        private IEnumerable<ICard> _twoPair;
        private IEnumerable<ICard> _pair;

        public CombinationHelper(IEnumerable<ICard> playerCards, IEnumerable<ICard> boardCards)
        {
            var cards = playerCards.Concat(boardCards);
            _dictByCardValue = GetSortedDictionaryByCardValue(cards);
            _dictByCardSuit = GetSortedDictionaryByCardSuit(cards);
            FindCombinationAndWinnerCards(cards);
        }

        private void FindCombinationAndWinnerCards(IEnumerable<ICard> cards)
        {
            if (IsRoyalFlush(cards))
            {
                WinnerCards = _royalFlush;
                Combination = PokerCombinations.RoyalFlush;
                return;
            }

            if (IsStraightFlush(cards))
            {
                WinnerCards = _straightFlush;
                Combination = PokerCombinations.StraightFlush;
                return;
            }

            if (IsFourOfAKind(cards))
            {
                WinnerCards = _fourOfAKind;
                Combination = PokerCombinations.FourOfAKind;
                return;
            }

            if (IsFullHouse(cards))
            {
                WinnerCards = _fullHouse;
                Combination = PokerCombinations.FullHouse;
                return;
            }

            if (IsFlush(cards))
            {
                WinnerCards = _flush;
                Combination = PokerCombinations.Flush;
                return;
            }

            if (IsStraight(cards))
            {
                WinnerCards = _straight;
                Combination = PokerCombinations.Straight;
                return;
            }

            if (IsThreeOfAKind(cards))
            {
                WinnerCards = _royalFlush;
                Combination = PokerCombinations.ThreeOfAKind;
                return;
            }

            if (IsTwoPair(cards))
            {
                WinnerCards = _twoPair;
                Combination = PokerCombinations.TwoPair;
                return;
            }

            if (IsPair(cards))
            {
                WinnerCards = _pair;
                Combination = PokerCombinations.Pair;
                return;
            }

            WinnerCards = new[] { GetHighCard(cards) };
            Combination = PokerCombinations.HighCard;
        }

        private Dictionary<CardSuit, List<ICard>> GetSortedDictionaryByCardSuit(IEnumerable<ICard> cards)
        {
            var dict = new Dictionary<CardSuit, List<ICard>>();

            foreach (var card in cards)
            {
                if (!dict.ContainsKey(card.Suit))
                {
                    dict[card.Suit] = new List<ICard>();
                }

                dict[card.Suit].Add(card);
            }

            foreach (var kv in dict)
            {
                kv.Value.Sort(new CardValueComparer(OrderBy.Desc));
            }

            return dict;
        }

        private Dictionary<CardValue, List<ICard>> GetSortedDictionaryByCardValue(IEnumerable<ICard> cards)
        {
            var dict = new Dictionary<CardValue, List<ICard>>();

            foreach (var card in cards)
            {
                if (!dict.ContainsKey(card.Value))
                {
                    dict[card.Value] = new List<ICard>();
                }

                dict[card.Value].Add(card);
            }

            return dict.OrderByDescending(x => x.Key)
                       .ToDictionary(x => x.Key, x => x.Value);
        }

        internal ICard GetHighCard(IEnumerable<ICard> cards)
        {
            return cards.Max(new CardValueComparer());
        }

        private bool IsRoyalFlush(IEnumerable<ICard> cards)
        {
            if (!IsStraight(cards))
                return false;

            var royalFlushCardList = new List<ICard>()
            {
                new Card(CardValue.Ace, CardSuit.Hearts),
                new Card(CardValue.King, CardSuit.Hearts),
                new Card(CardValue.Queen, CardSuit.Hearts),
                new Card(CardValue.Jack, CardSuit.Hearts),
                new Card(CardValue.Ten, CardSuit.Hearts)
            };

            var royalFlushPlayerCards = cards?.Distinct(new CardValueComparer())
                                              .ToList()
                                              .Select(x => x)
                                              .Intersect(royalFlushCardList, new CardValueComparer());

            if (royalFlushPlayerCards?.Count() == 5)
            {
                _royalFlush = royalFlushPlayerCards;
                return true;
            }

            return false;
        }

        private bool IsStraightFlush(IEnumerable<ICard> cards)
        {
            if (!IsFlush(cards))
                return false;

            if (IsStraight(_flush))
            {
                _straightFlush = _flush;
                return true;
            }

            return false;
        }

        private bool IsStraight(IEnumerable<ICard> cards)
        {
            if (cards.Count() < 5)
                return false;

            var inputCardList = cards.ToList();
            inputCardList = inputCardList.Distinct(new CardValueComparer()).ToList();
            inputCardList.Sort(new CardValueComparer(OrderBy.Desc));

            for (int i = 0; i < inputCardList.Count;)
            {
                var matchList = new List<ICard>();
                matchList.Add(inputCardList[i]);

                int j = 1;
                while (inputCardList.Any(card => card.Value == inputCardList[i].Value - j))
                {
                    matchList.Add(inputCardList.First(card => card.Value == inputCardList[i].Value - j));
                    j++;
                }

                i += j;
                if (matchList.Count >= 5)
                {
                    _straight = matchList;
                    return true;
                }
            }

            return false;
        }

        private bool IsFourOfAKind(IEnumerable<ICard> cards)
        {
            var fourCards = TryGetOneValueCards(4);

            if (fourCards != null)
            {
                _fourOfAKind = fourCards;
                return true;
            }

            return false;
        }

        private bool IsFullHouse(IEnumerable<ICard> cards)
        {
            if (IsThreeOfAKind(cards) && IsPair(cards))
            {
                _fullHouse = _threeOfAKind.Concat(_pair);
                return true;
            }

            return false;
        }

        private bool IsFlush(IEnumerable<ICard> cards)
        {
            var flushCards = TryGetOneSuitCards(5);

            if (flushCards != null)
            {
                _flush = flushCards;
                return true;
            }

            return false;
        }

        private bool IsThreeOfAKind(IEnumerable<ICard> cards)
        {
            var threeCards = TryGetOneValueCards(3);

            if (threeCards != null)
            {
                _threeOfAKind = threeCards;
                return true;
            }

            return false;
        }

        private bool IsPair(IEnumerable<ICard> cards)
        {
            var twoCards = TryGetOneValueCards(2);

            if (twoCards != null)
            {
                _pair = twoCards;
                return true;
            }

            return false;
        }

        private bool IsTwoPair(IEnumerable<ICard> cards)
        {
            if (_pair == null)
                return false;

            var secondPair = TryGetOneValueCards(2);

            if (secondPair != null)
            {
                _twoPair = _pair.Concat(secondPair);
                return true;
            }

            return false;
        }

        private IEnumerable<ICard>? TryGetOneValueCards(int oneValueCardsCount)
        {
            if (_dictByCardValue.Any(kv => kv.Value.Count() >= oneValueCardsCount))
            {
                var valueAndCards = _dictByCardValue.First(kv => kv.Value.Count() >= oneValueCardsCount);
                _dictByCardValue.Remove(valueAndCards.Key);
                return valueAndCards.Value;
            }

            return null;
        }

        private IEnumerable<ICard>? TryGetOneSuitCards(int oneValueCardsCount)
        {
            if (_dictByCardSuit.Any(kv => kv.Value.Count() >= oneValueCardsCount))
            {
                var valueAndCards = _dictByCardSuit.First(kv => kv.Value.Count() >= oneValueCardsCount);
                _dictByCardSuit.Remove(valueAndCards.Key);
                return valueAndCards.Value;
            }

            return null;
        }

        internal IPlayer TryGetSecondWinnerHighCard(IEnumerable<ICard> winnerCards)
        {
            throw new NotImplementedException();
        }
    }
}