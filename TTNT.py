import heapq  # Thư viện dùng để tạo hàng đợi ưu tiên cho thuật toán A*
import copy  # Dùng để sao chép trạng thái trò chơi
import pygame  # Thư viện để tạo giao diện đồ họa
from pygame.locals import *  # Các hằng số của pygame
from OpenGL.GL import *  # Thư viện OpenGL để vẽ 3D
from OpenGL.GLU import *  # Thư viện hỗ trợ OpenGL
import math  # Dùng để tính toán hình học

class RingSortingGame3D:
    def __init__(self, initial_state):
        self.initial_state = initial_state
        self.num_columns = len(initial_state)
        self.current_state = copy.deepcopy(initial_state)
        self.solution_moves = []
        self.colors = {"red": (1, 0, 0), "blue": (0, 0, 1), "green": (0, 1, 0)}
        self.init_pygame()
        self.solve_with_astar()
        self.run_game()
    
    def init_pygame(self):
        pygame.init()
        self.display = (800, 600)
        pygame.display.set_mode(self.display, DOUBLEBUF | OPENGL)
        gluPerspective(45, (self.display[0] / self.display[1]), 0.1, 50.0)
        glTranslatef(-3, -2, -10)
    
    def is_goal(self, state):
        return all(len(set(column)) <= 1 and len(column) in [0, 4] for column in state)
    
    def get_possible_moves(self, state):
        moves = []
        for i in range(self.num_columns):
            if not state[i]:
                continue
            for j in range(self.num_columns):
                if i != j and (not state[j] or state[j][-1] == state[i][-1]):
                    moves.append((i, j))
        return moves
    
    def move(self, state, from_col, to_col):
        new_state = copy.deepcopy(state)
        rings_to_move = []
        while new_state[from_col] and (not rings_to_move or new_state[from_col][-1] == rings_to_move[-1]):
            rings_to_move.append(new_state[from_col].pop())
        while rings_to_move:
            new_state[to_col].append(rings_to_move.pop())
        return new_state
    
    def heuristic(self, state):
        misplaced_rings = 0
        for column in state:
            if column:
                color = column[0]
                misplaced_rings += sum(1 for ring in column if ring != color)
        return misplaced_rings
    
    def solve_with_astar(self):
        pq = []
        heapq.heappush(pq, (0, self.current_state, []))
        visited = set()
        
        while pq:
            _, state, path = heapq.heappop(pq)
            state_tuple = tuple(tuple(col) for col in state)
            
            if state_tuple in visited:
                continue
            visited.add(state_tuple)
            
            if self.is_goal(state):
                self.solution_moves = path
                return
            
            for move in self.get_possible_moves(state):
                new_state = self.move(state, *move)
                new_path = path + [move]
                cost = len(new_path) + self.heuristic(new_state)
                heapq.heappush(pq, (cost, new_state, new_path))
    
    def execute_solution(self):
        if self.solution_moves:
            from_col, to_col = self.solution_moves.pop(0)
            self.current_state = self.move(self.current_state, from_col, to_col)
    
    def draw_ring(self, x, y, color):
        glColor3fv(color)
        glPushMatrix()
        glTranslatef(x, y, 0)
        glBegin(GL_TRIANGLE_FAN)
        glVertex3f(0, 0, 0)
        for i in range(0, 361, 10):
            angle = math.radians(i)
            glVertex3f(0.4 * math.cos(angle), 0.4 * math.sin(angle), 0)
        glEnd()
        glPopMatrix()
    
    def draw_scene(self):
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        for i, column in enumerate(self.current_state):
            for j, ring in enumerate(column):
                color = self.colors.get(ring, (1, 1, 1))
                self.draw_ring(i * 2, j * 0.8, color)
        pygame.display.flip()
    
    def run_game(self):
        running = True
        clock = pygame.time.Clock()
        while running:
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
            self.execute_solution()
            self.draw_scene()
            clock.tick(1)
        pygame.quit()

if __name__ == "__main__":
    initial_state = [["red", "blue", "green", "red"], ["green", "blue", "red", "green"], ["blue", "green", "blue", "red"], [], []]
    game = RingSortingGame3D(initial_state)