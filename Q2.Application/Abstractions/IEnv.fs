namespace Q2.Application

type IEnv =
    inherit IHttp
    inherit IPersistence
    inherit ISecrets
    inherit ITime
