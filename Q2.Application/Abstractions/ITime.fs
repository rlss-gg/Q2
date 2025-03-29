namespace Q2.Application

open System

type ITime =
    abstract GetCurrentTime: unit -> DateTime
