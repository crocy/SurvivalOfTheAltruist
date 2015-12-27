# load csv data

# library(ggplot2)

loadCsvData <- function() {
  print("Loading files...")
  csvFiles <- list.files("data", pattern = "*.csv", full.names = TRUE)

  for (csvFile in csvFiles) {
    print(paste("Loading:", csvFile))

    if (!exists("tempDataAll")) {
      tempDataAll = read.csv(csvFile, sep = ";", na.strings = "")
    } else {
      tempDataAll = rbind(tempDataAll, read.csv(csvFile, sep = ";", na.strings = ""))
    }
  }

  print("Done.")
  return(tempDataAll)
}

filterDataPerGroups <- function(testData) {
  genD <- testData[testData$generator == "D", ]$lifetime
  genA <- testData[testData$generator == "A", ]$lifetime
  genS <- testData[testData$generator == "S", ]$lifetime

  result <- data.frame(genD, genA, genS)
  names(result) <- c("genD", "genA", "genS")
  return(result)
}

runTTest <- function(dataAll1, dataAll2, pa = FALSE, ve = FALSE) {
  a <- filterDataPerGroups(dataAll1)
  b <- filterDataPerGroups(dataAll2)

  print("t-test for generator D")
  print(t.test(a$genD, b$genD, paired = pa, var.equal = ve))

  print("t-test for generator A")
  print(t.test(a$genA, b$genA, paired = pa, var.equal = ve))

  print("t-test for generator S")
  print(t.test(a$genS, b$genS, paired = pa, var.equal = ve))
}

varsToData <- function() {
  print("starting...")
  l = grep("esc", ls(.GlobalEnv))
  print(l)
  returnData = NULL
  sprintf("list = %s", l)

  for (n in l) {
    sprintf("name = %i", n)
    print(n)
    s = ls(.GlobalEnv)[n]
    sprintf("item = %s", names(s))
    print(s)
    run = substr(s, 5, 10)
    sprintf("run = %s", run)
    print(run)
    temp = get(s)
    temp$run <- run
    returnData = rbind(returnData, temp)
  }

  return(returnData)
}
