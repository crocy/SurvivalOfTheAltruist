library(ggplot2)

plotAltruismToLifetime <- function(data) {
  qplot(altruism, lifetime, data = data, color = generator)
}

savePlotsP <- function(dataList, folder = "./", plotFunct, facet = NULL) {
  folder = paste("plots/generated/", folder, "_", plotFunct$labels[["x"]], "-", plotFunct$labels[["y"]], "/", sep = "")
  print(paste("Saving plots to:", folder))
  if (!dir.exists(folder)) {
    dir.create(folder)
  }

  for (i in 1:length(dataList)) {
    print(paste("Generating plot:", names(dataList[i])))

    plotFunct$data = dataList[[i]]
    png(paste(folder, names(dataList[i]), ".png", sep = ""), width = 600, height = 400)
    plot(plotFunct + facet)
    dev.off()
  }
}

# dataAll = loadCsvData()

qplot(lifetime, data = dataAll, color = groupTag, binwidth = 5)
qplot(lifetime, data = dataAll, fill = generator, size = groupTag, binwidth = 5)
qplot(lifetime, data = dataAll, color = groupTag, geom = "density")
qplot(altruism, data = dataAll, binwidth = 0.01, fill = groupTag, geom = "bar")
qplot(speedBase, data = dataAll, binwidth = 0.1, fill = groupTag, geom = "bar")
qplot(speedBase, data = dataAll, color = groupTag, geom = "density")

qplot(altruism, lifetime, data = dataAll, color = groupTag, geom = "density2d")
qplot(altruism, lifetime, data = dataAll, color = groupTag, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, color = generator, geom = c("smooth", "point"))
qplot(altruism, lifetime, data = dataAll, color = generator, geom = c("smooth", "point"), size = energyShared)
qplot(altruism, lifetime, data = dataAll, facets = . ~ generator, geom = "density2d", color = generator)

qplot(altruism, lifetime, data = dataAllR, color = generator, geom = "smooth", facets = esc ~ generator)
qplot(altruism, lifetime, data = dataAllA, color = gR, geom = "smooth", facets = esc ~ generator)
qplot(altruism, lifetime, data = subset(dataAllA, run != "r30"), color = generator, geom = "smooth", facets = esc ~ generator, group = groupTag)

qplot(altruism, lifetime, data = dataAll, color = generator, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, color = generator, geom = "smooth", shape = groupTag)
qplot(altruism, lifetime, data = dataAll, fill = generator, geom = "smooth", shape = groupTag)
qplot(speedBase, energyCollected, data = esc2a2, color = generator, geom = c("smooth", "point"), size = energyCollected, alpha = energyCollected)
qplot(altruism, lifetime, data = dataAll, color = generator, geom = "smooth", facets = esc ~ generator)

qplot(altruism, lifetime, data = esc1a2, color = generator, geom = c("smooth", "point"), alpha = energyShared, size = energyShared)

qplot(altruism, lifetime, data = esc1a2, geom = "boxplot", fill = groupTag)
qplot(altruism, lifetime, data = esc1a3, geom = "boxplot", facets = . ~ generator, fill = groupTag)

qplot(altruism, lifetime, data = dataAll, geom = "violin", fill = groupTag, facets = esc ~ generator)


# savePlotsP(dataList, "density", qplot(lifetime, geom = "density", color = groupTag))
# savePlotsP(dataList, "density2d-facet", qplot(altruism, lifetime, geom = "density2d", color = groupTag), facet_grid(. ~ generator))
#
# savePlotsP(dataList, "boxplot", qplot(altruism, lifetime, geom = "boxplot", fill = groupTag))
# savePlotsP(dataList, "boxplot-facet", qplot(altruism, lifetime, geom = "boxplot", fill = groupTag), facet_grid(. ~ generator))
# savePlotsP(dataList, "boxplot-facet", qplot(speedBase, lifetime, geom = "boxplot", fill = generator), facet_grid(. ~ generator))
#
# savePlotsP(dataList, "violin-facet", qplot(altruism, lifetime, geom = "violin", fill = groupTag), facet_grid(. ~ generator))
#
# savePlotsP(dataList, "smooth", qplot(altruism, lifetime, geom = "smooth", color = generator))
# savePlotsP(dataList, "smooth", qplot(speedBase, lifetime, geom = "smooth", color = generator))
# savePlotsP(dataList, "smooth", qplot(speedBase, lifetime, geom = "smooth", color = generator))
# savePlotsP(dataList, "smooth", qplot(speedBase, energyCollected, geom = "smooth", color = generator))
# savePlotsP(dataList, "smooth-groupTag", qplot(speedBase, energyCollected, geom = "smooth", color = groupTag))


ggplot(dataAll, aes(esc, lifetime, group=groupTag)) +
  facet_grid(groupTag ~ generator) +
  geom_point(alpha=1/50, position="jitter") +
  geom_smooth(method="lm", formula=y~x)

ggplot(dataAll, aes(groupTag, lifetime, group=esc)) +
  facet_grid(esc ~ generator) +
  geom_point(alpha=1/50, position="jitter") +
  geom_smooth(method="lm", formula=y~x)

ggplot(subset(dataAllA, run != "r30"), aes(altruism, lifetime)) +
  facet_grid(esc ~ generator) +
  geom_point(alpha=1/50, position="jitter", aes(color = groupTag)) +
  geom_smooth(method="lm", formula=y~x, size = 1)
