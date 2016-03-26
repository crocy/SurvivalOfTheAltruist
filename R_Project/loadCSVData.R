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

summ <- function(selectedCol) {
  for (run in vecRun) {
    for (esc in vecEsc) {
      # print(paste("Summary for: run = ", run, ", esc = ", esc))
      tempSum = summary(dataAllA[dataAllA$run == run & dataAllA$esc == esc, selectedCol])
      temp = NULL
      for (i in 1:6) {
        temp = paste(temp, "&", tempSum[[i]])
      }
      temp = paste(temp, "\\")
      print(temp)
    }
  }
}

### Pearsons correlation
# x = dataAll[dataAll$generator == "A", ]
# cor(x$lifetime, x$esc, method = "pearson")

### 1: tab:cor_altruism-lifetime_generator-esc
# corForData(dataAll, w = "esc")

### 2: tab:cor_esc-lifetime_generator-groupTag
# corForData(dataAll, x = "esc")

### 3: tab:cor_altruism-lifetime_generator-esc_1024w-genEF
# corForData(dataAllEF, w = "esc")

### 4: tab:cor_altruism-lifetime_generator-esc_gR
# corForData(dataAllR, w = "esc")

### 5: tab:cor_esc-lifetime_generator-groupTag_gR
# corForData(dataAllR, x = "esc")

### 6: tab:cor_altruism-lifetime_generator-esc_1024w-genEF-gR
# corForData(dataAllREF, w = "esc")

### 7: tab:cor_smooth-point-facet_speedBase-lifetime_generator
# corForData(dataAll, x = "speedBase")

### 8: tab:cor_smooth-jitter-facet_speedBase-energyCollected_generator
# corForData(dataAll, x = "speedBase", y = "energyCollected")

corForData <- function(data, x = "altruism", y = "lifetime", z = "generator", w = "groupTag", additionalStat = NULL) {
  output = ""
  outputP = ""
  outMean = ""
  outMedian = ""

  for (i in levels(data[[w]])) {
    skippedLevel = FALSE
    # cat("level: i =", i, "\n")
    for (j in levels(data[[z]])) {
      # cat("level: j =", j, "\n")
      tmpData = data[data[[w]] == i & data[[z]] == j, ]
      if (length(tmpData[, 1]) == 0) {
        skippedLevel = TRUE
        cat("Skipping level since no data found for:", w, "=", i, ",", z, "=", j, "\n")
        next
      }
      c = cor.test(as.numeric(tmpData[[x]]), as.numeric(tmpData[[y]]), method = "pearson", alternative = "two.sided")
      # print(c)
      tmpCor = format(c$estimate, digits = 2, width = 7)
      tmpP = format(c$p.value, digits = 2, width = 7, trim = T)
      cat(w, ": ", i, ", ", z, ": ", j, ", cor = ", tmpCor, ", p-value = ", tmpP, ", n = ", (c$parameter + 2), "\n", sep = "")
      output = paste(output, "&", tmpCor)
      outputP = paste(outputP, " & ", tmpP, sep = "")

      if (!is.null(additionalStat)) {
        meanA = format(mean(tmpData[[additionalStat]]), digits = 2, nsmall = 1)
        medianA = format(median(tmpData[[additionalStat]]), digits = 2, nsmall = 1)
        cat("  additional stat: mean(", additionalStat, ") = ", meanA, ", median(", additionalStat, ") = ", medianA, "\n", sep = "")
        outMean = paste(outMean, "&", meanA)
        outMedian = paste(outMedian, "&", medianA)
      }
    }

    if (skippedLevel) {
      next
    }

    output = paste(output, "\\\\ \n")
    outputP = paste(outputP, "\\\\ \n")

    if (!is.null(additionalStat)) {
      outMean = paste(outMean, "\\\\ \n")
      outMedian = paste(outMedian, "\\\\ \n")
    }
  }

  cat("\nLaTeX tabular output:", x, "vs.", y, "; for:", z, "vs.", w, "\n")
  cat(output)
  cat("\np-values output:\n")
  cat(outputP)

  if (!is.null(additionalStat)) {
    cat("\nAdditional statistic output for:", additionalStat, "\n--------------------------------")
    cat("\nmean", additionalStat, "output:\n")
    cat(outMean)
    cat("\nmedian", additionalStat, "output:\n")
    cat(outMedian)
  }
}
