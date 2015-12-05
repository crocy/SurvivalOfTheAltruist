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