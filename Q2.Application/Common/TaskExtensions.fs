namespace System.Threading.Tasks

module Task =
    let map f v = task {
        let! v' = v
        return f v'
    }

    let bind f v = task {
        let! v' = v
        return! f v'
    }

    let lift v = task {
        return v
    }
