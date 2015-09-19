library(ggplot2)

plotAltruismToLifetime <- function(data) {
  qplot(altruism, lifetime, data = data, color = generator)
}

qplot(lifetime, data = dataAll, color = groupTag, binwidth = 5)
qplot(lifetime, data = dataAll, fill = generator, size = groupTag, binwidth = 5)
qplot(lifetime, data = dataAll, color = groupTag, geom = "density")
qplot(altruism, lifetime, data = dataAll, color = groupTag, geom = "density2d")
qplot(altruism, lifetime, data = dataAll, color = groupTag, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, color = generator, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, geom = "smooth")
qplot(altruism, lifetime, data = dataAll, color = generator, geom = c("smooth", "point"))
