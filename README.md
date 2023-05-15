# Custom WinForms Spreadsheet Application Inspired by Microsoft Excel

## Name
Andrew J. Lefors 


## Description
This application was designed using Test Driven Development (TDD) and is Event-Driven. Each nodetype is dynamically loaded for future plugins of different operator types.
Expressions are stored in an expression tree and converted to Polish notation to preserve operator precedence. Complex operations are allowed and assignment of variables for new expressions.
The contents of cells referencing other cells are updated as the referenced cells value changes, using events on the backend to update the front-end display. Cell text is preserved while cell value is displayed,
with the text displayed while editing a cell. OVerall, the application emulates the base functionality of Microsfot Excel and demonstrates modular, iterative design utilizing Events and Test Driven development 
produce a moderate sized cohesive application.

## License
This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

See <https://www.gnu.org/licenses/> for a copy of the License. 
