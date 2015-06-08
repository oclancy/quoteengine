namespace QuoteEngine

open System.Collections.Generic

type Position =
    {
        Broker:string
        Symbol:string
        Isin:string
        Exchange:string

        SellQuantity:int
        BuyQuantity:int

        DiscountMap: Dictionary<int,decimal>
        ClientDiscountMap: Dictionary<string,int>

        AutoRefresh:bool
    }
