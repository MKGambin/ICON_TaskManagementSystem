import './App.css';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import TaskItemForm from './pages/taskItems/TaskItemForm';
import TaskItemList from './pages/taskItems/TaskItemList';
import UserForm from './pages/users/UserForm';
import UserList from './pages/users/UserList';

function App() {
    return (
        <Router>
            <Routes>
                {/* TaskItems routes */}
                <Route path="/taskitems/:userId" element={<TaskItemList />} />
                <Route path="/taskitems/create/:userId" element={<TaskItemForm />} />
                <Route path="/taskitems/update/:userId/:id" element={<TaskItemForm />} />

                {/* Users routes */}
                <Route path="/users" element={<UserList />} />
                <Route path="/users/create" element={<UserForm />} />
                <Route path="/users/update/:id" element={<UserForm />} />

                {/* default to User route */}
                <Route path="*" element={<UserList />} />
            </Routes>
        </Router>
    );
}

export default App;
