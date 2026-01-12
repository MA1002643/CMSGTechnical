

## CMSG Technical Assessment


This test is designed to test a broad scope of programming skills and capacities. Your artisitc capacity as a developer are not being tested - although if you wish to show them off then you are very welcome to.

This is an open ended task that could consume you for days however I would recommend only spending 2 hours on this.


### Background

You have been tasked with bringing a e-commerce platform up to MVC. As part of the development team you are focusing on the menu and basket areas. It can be assumed that the user has already authenticated, and that the responses from the Database items that the end user is allowed to see and edit.

Below are a number of "tickets" that are needed to bring this site up to "MVP". These are a mix of *features* and a number of obvious and subtle *bugs*. If you choose to ignore a bug please state the reason.


### Feature Tickets
- The basket needs to persist between page loads
- The basket menu needs to be responsive to fit on a mobile device
- Menu items need to stand out when hovering over
- Menu items need a category, and this needs to be reflected in the UI (the categories themselves are arbitary. ie: Starter, Main, Dessert)
- The restuarant always add £2 as a delivery fee, this needs to be included in the basket total
- The basket needs to group the items together using a quanity display
- Code coverage currently sits at 0%

### Bug Tickets (sent in by QA)
- When I add something to my basket on one tab, it doesn't seem to add them on the other after I refresh?
- The menu items aren't displayed in price order
- Chocolate Cake's description isn't showing
- The basket doesn't show £
- The basket total doesn't add up correctly