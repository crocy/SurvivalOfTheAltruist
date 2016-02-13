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

### astetics

fs_atitle = 23 # font size: axis title
fs_atext = 17 # font size: axis title
fs_l = 17 # font size: legend
fs_strip = 20 # font size: strip/facets

themeNoLegend <- theme(
  legend.position="none",
  axis.title.x = element_text(size = fs_atitle),
  axis.title.y = element_text(size = fs_atitle),
  axis.text.x = element_text(size = fs_atext),
  axis.text.y = element_text(size = fs_atext),
  strip.text.x = element_text(size = fs_strip),
  strip.text.y = element_text(size = fs_strip)
)

themeLegend <- theme(
  legend.position="bottom",
  legend.title = element_text(size = fs_l),
  legend.text = element_text(size = fs_l),
  axis.title.x = element_text(size = fs_atitle),
  axis.title.y = element_text(size = fs_atitle),
  axis.text.x = element_text(size = fs_atext),
  axis.text.y = element_text(size = fs_atext),
  strip.text.x = element_text(size = fs_strip),
  strip.text.y = element_text(size = fs_strip)
)

themeNoYStrip <- theme(
  legend.position="bottom",
  legend.title = element_text(size = fs_l),
  legend.text = element_text(size = fs_l),
  axis.title.x = element_text(size = fs_atitle),
  axis.title.y = element_text(size = fs_atitle),
  axis.text.x = element_text(size = fs_atext),
  axis.text.y = element_text(size = fs_atext),
  strip.text.x = element_text(size = fs_strip),
  strip.text.y = element_blank()
)

labelsGroups <- c(expression("G"["S"]), expression("G"["DS"]), expression("G"["DA"]), expression("G"["A"]))
labelsGroupsTable <- c("A_0-25" = expression("G"["S"]), "A_25-50" = expression("G"["DS"]), "A_50-75" = expression("G"["DA"]), "A_75-100" = expression("G"["A"]))

# legendGroups <- scale_fill_discrete(name = "Skupine:", labels = labelsGroups)
legendGroups <- scale_fill_hue(name = "Skupine:", labels = labelsGroups)
legendGroupsColor <- scale_color_hue(name = "Skupine:", labels = labelsGroups)



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

##################
# graphs
##################
### save plot
# png("plot.png", width = 1024, height = 800)
dev.copy(png, "plot.png", width = 1024, height = 800); dev.off()

### 1
qplot(altruism, lifetime, data = dataAll, geom = "violin", fill = groupTag, facets = esc ~ generator) +
  xlab("Stopnja altruizma") + ylab("Življenska doba") +
  legendGroups +
  themeLegend

### 2
ggplot(dataAll, aes(altruism, lifetime)) +
  facet_grid(esc ~ generator) +
  geom_point(alpha = 1/15, position = "jitter", aes(color = groupTag)) +
  geom_smooth(method = "lm", formula = y ~ x, size = 1) +
  xlab("Stopnja altruizma") + ylab("Življenska doba") +
  guides(colour = guide_legend(override.aes = list(alpha = 1))) +
  legendGroupsColor +
  themeLegend

### 3
ggplot(dataAll, aes(esc, lifetime)) +
  facet_grid(groupTag ~ generator) +
  geom_point(alpha = 1/15, position = "jitter", aes(color = groupTag)) +
  geom_smooth(method = "lm", formula = y ~ x, size = 1) +
  xlab("ESC") + ylab("Življenska doba") +
  guides(colour = guide_legend(override.aes = list(alpha = 1))) +
  legendGroupsColor +
  themeNoYStrip
# ggplot(dataAll, aes(esc, lifetime)) +
#   facet_grid(groupTag ~ generator) +
#   geom_point(alpha = 1/50, position = "jitter", aes(color = groupTag)) +
#   geom_smooth(method = "lm", formula = y ~ x, size = 1)

### none
# ggplot(dataAll, aes(groupTag, lifetime, group = esc)) +
#   facet_grid(esc ~ generator)       +
#   geom_point(alpha = 1/50, position="jitter", aes(color = groupTag)) +
#   geom_smooth(method = "lm", formula = y ~ x, size = 1)

### 4
ggplot(dataAllR, aes(altruism, lifetime)) +
  facet_grid(esc ~ generator) +
  geom_point(alpha = 1/10, position = "jitter") +
  scale_fill_discrete("Skupine") +
  geom_smooth(method = "lm", formula = y ~ x, size = 1) +
  xlab("Stopnja altruizma") + ylab("Življenska doba") +
  themeNoLegend
# ggplot(dataAll, aes(altruism, lifetime)) +
#   facet_grid(esc ~ generator) +
#   geom_point(alpha = 1/50, position = "jitter", aes(color = groupTag)) +
#   geom_smooth(method = "lm", formula = y ~ x, size = 1)

### 5
ggplot(dataAllR, aes(esc, lifetime)) +
  facet_grid(. ~ generator) +
  geom_point(alpha = 1/10, position = "jitter") +
  geom_smooth(method = "lm", formula = y ~ x, size = 1) +
  xlab("ESC") + ylab("Življenska doba") +
  themeNoLegend

### 6
ggplot(dataAll, aes(speedBase, lifetime)) +
  geom_point(aes(color = generator), alpha = 1/50) +
  facet_grid(. ~ generator) +
  geom_smooth() +
  xlab("Hitrost premikanja") + ylab("Življenska doba") +
  themeNoLegend

### 7
ggplot(dataAll, aes(speedBase, energyCollected)) +
  geom_jitter(aes(color = generator), alpha = 1/25) +
  facet_grid(. ~ generator) +
  geom_smooth(size = 1) +
  coord_cartesian(ylim = c(0, 15)) +
  xlab("Hitrost premikanja") + ylab("Število nabranih virov energije") +
  themeNoLegend
