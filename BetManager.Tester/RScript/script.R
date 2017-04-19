# pomocna funkce pro predani dat ze C#
CreateDataFrame <- function(names, homeTeam, awayTeam, FTHG, FTAG, dates) {
    x <- data.frame(homeTeam, awayTeam, FTHG, FTAG, dates)
    colnames(x) <- names
    return(x)
}

# priprava dat
DCmodelData <- function(df) {
    hm <- model.matrix( ~ HomeTeam - 1, data = df, contrasts.arg = list(HomeTeam = 'contr.treatment'))
    am <- model.matrix( ~ AwayTeam - 1, data = df)
    team.names <- unique(c(levels(df$HomeTeam), levels(df$AwayTeam)))
    return(list(
        homeTeamDM = hm,
        awayTeamDM = am,
        homeGoals = df$FTHG,
        awayGoals = df$FTAG,
        dates = as.Date(df$Date), # parse dateformat! vychozi je %Y-%m-%d
        teams = team.names
    ))
}

# funkce tau
tau <- Vectorize(function(xx, yy, lambda, mu, rho) {
    if (xx == 0 & yy == 0) {
        return(1 - (lambda * mu * rho))
    } else if (xx == 0 & yy == 1) {
        return(1 + (lambda * rho))
    } else if (xx == 1 & yy == 0) {
        return(1 + (mu * rho))
    } else if (xx == 1 & yy == 1) {
        return(1 - rho)
    } else { return(1) }
    })

# likehood
DClogLik <- function(y1, y2, lambda, mu, rho = 0, w = NULL) {
    #rho=0, independence
    #y1: home goals
    #y2: away goals
    t <- tau(y1, y2, lambda, mu, rho)
    t[t <= 0] <- 0.0001
    loglik <- log(t) + log(dpois(y1, lambda)) + log(dpois(y2, mu))
    if (is.null(w)) {
        return(sum(loglik))
    } else {
        return(sum(loglik * w))
    }
}

# vahy dle data
DCweights <- function(dates, currentDate, ksi = 0) {
    datediffs <- dates - as.Date(currentDate)
    datediffs <- as.numeric(datediffs) * -1
    datediffs[datediffs <= 0] <- 0 # Future dates should have zero weights
    w <- exp(-1 * ksi * datediffs)
    return(w)
}

# optimalizace
DCoptimFn <- function(params, DCm, ksi = 0, currentDate) {

    home.p <- params[1]
    rho.p <- params[2]

    nteams <- length(DCm$teams)
    attack.p <- matrix(params[3:(nteams + 2)], ncol = 1)
    defence.p <- matrix(params[(nteams + 3):length(params)], ncol = 1)

    lambda <- exp(DCm$homeTeamDM %*% attack.p + DCm$awayTeamDM %*% defence.p + home.p)
    mu <- exp(DCm$awayTeamDM %*% attack.p + DCm$homeTeamDM %*% defence.p)

    # w <- NULL
    w <- DCweights(DCm$dates, ksi = ksi, currentDate = currentDate)
    return(
        DClogLik(y1 = DCm$homeGoals, y2 = DCm$awayGoals, lambda, mu, rho.p, w = w) * -1
    )
}

# podminka nulovosti souctu utoku
DCattackConstr <- function(params, DCm, ...) {
    nteams <- length(DCm$teams)
    attack.p <- matrix(params[3:(nteams + 2)], ncol = 1)
    # defence.p <- matrix(params[(nteams + 3):length(params)], ncol = 1)
    # return(((sum(attack.p) / nteams)-(sum(defence.p) / nteams))-2)
    return((sum(attack.p) / nteams) - 1)
}

# odhad
doPrediction <- function(ksi = 0, currentDate) {
    #initial parameter estimates
    attack.params <- rep(.01, times = nlevels(dta$HomeTeam))
    defence.params <- rep(-0.08, times = nlevels(dta$HomeTeam))
    home.param <- 0.06
    rho.init <- 0.3
    par.inits <- c(home.param, rho.init, attack.params, defence.params)
    #it is also usefull to give the parameters some informative names
    names(par.inits) <- c('HOME', 'RHO', paste('Attack', dcm$teams, sep = '.'), paste('Defence', dcm$teams, sep = '.'))
    library(numDeriv)
    library(alabama)
    res <- auglag(par = par.inits, fn = DCoptimFn, heq = DCattackConstr, DCm = dcm, ksi = ksi, currentDate = currentDate)
    return(res)
}

#
predict.result <- function(res, home, away, maxgoal = 10) {
    attack.home <- paste("Attack", home, sep = ".")
    attack.away <- paste("Attack", away, sep = ".")
    defence.home <- paste("Defence", home, sep = ".")
    defence.away <- paste("Defence", away, sep = ".")

    # Expected goals home
    lambda <- exp(res$par['HOME'] + res$par[attack.home] + res$par[defence.away])
    # Expected goals away
    mu <- exp(res$par[attack.away] + res$par[defence.home])

    probability_matrix <- dpois(0:maxgoal, lambda) %*% t(dpois(0:maxgoal, mu))

    scaling_matrix <- matrix(tau(c(0, 1, 0, 1), c(0, 0, 1, 1), lambda, mu, res$par['RHO']), nrow = 2)
    probability_matrix[1:2, 1:2] <- probability_matrix[1:2, 1:2] * scaling_matrix

    HomeWinProbability <- sum(probability_matrix[lower.tri(probability_matrix)])
    DrawProbability <- sum(diag(probability_matrix))
    AwayWinProbability <- sum(probability_matrix[upper.tri(probability_matrix)])

    return(c(HomeWinProbability, DrawProbability, AwayWinProbability))
}

#predict.summary <- function(res) {
    #smm <- 0
    #predict.result(res, dta$HomeTeam, dta$AwayTeam)
    #dta$HomeTeam

    #for (i in length(dta)) {
        #dta[i]$Date
    #}

    #return (res)
#}

#dta <- read.csv('E0.csv')
#dcm <- DCmodelData(dta)

#res <- doPrediction(ksi = 0.0065 / 3.5, currentDate = as.Date('2012-05-13'))
#write.csv(res$par, file = "par.csv")

#predict.result(res, "Arsenal", "Liverpool")
