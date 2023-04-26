import './App.css';
import Navbar from './Components/Navbar';
import Sidebar from './Components/Sidebar';
import Footer from './Components/Footer';
import Layout from './Pages/Layout';
import Index from './Pages/Index';
import About from './Pages/About';
import Contact from './Pages/Contact';
import RSS from './Pages/Rss';

import{
  BrowserRouter as Router,
  Routes,
  Route,
} from 'react-router-dom';


function App() {
  return (
    <div>
      <Router>
                <Routes>
                  <Route path='/' element={<Layout/>}>
                    <Route path='/' element={<Index />}/>
                    <Route path='blog' element={<Index />}/>
                    <Route path='blog/Contact' element={<Contact />}/>
                    <Route path='blog/About' element={<About />}/>
                    <Route path='blog/RSS' element={<RSS />}/>
                  </Route>
                </Routes>
        <Footer />
      </Router>
    </div>
  );
}

export default App;
