totalItems = 0
clothingItems = 0
for line in cart.LineItems:
  line.Discount = 0.0
  totalItems = totalItems + line.Quantity
  if line.Product.Category == 'Clothing':
    clothingItems = clothingItems + line.Quantity

clothingDiscount = 0.0
if clothingItems > 5:
  clothingDiscount = 0.11
elif clothingItems >= 2:
  clothingDiscount = 0.05

for line in cart.LineItems:
  if line.Product.Category == 'Clothing':
    line.Discount = clothingDiscount
  if totalItems >= 7:
    line.Discount = line.Discount + 0.03
