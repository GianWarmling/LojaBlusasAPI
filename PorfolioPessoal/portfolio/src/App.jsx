import { Route, Routes } from "react-router-dom"
import Navbar from "./components/Navbar"
import Home from "./pages/Home"
import About from "./pages/About"
import Projects from "./pages/Projects"
import Contacts from "./pages/Contacts"

function App() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="/sobre" element={<About />}/>
        <Route path="/projetos" element={<Projects />}/>
        <Route path="/contato" element={<Contacts />}/>
      </Routes>
    </>
  )
}

export default App
