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

