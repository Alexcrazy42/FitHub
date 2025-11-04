import { createContext } from 'react';
import { ThemeContextType } from './ThemeProvider';


export const ThemeContext = createContext<ThemeContextType | undefined>(undefined);
