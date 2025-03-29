namespace Q2.Application

type IEnv =
    inherit IDurable
    inherit IPersistence
    inherit ITime
